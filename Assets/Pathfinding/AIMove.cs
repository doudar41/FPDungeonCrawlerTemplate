using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class AIMove : MonoBehaviour
{

    enum States
    {
        Patrol,
        Chase,
        Melee
    }

    Transform player;
    Vector3 lastPlayerPosition;
    [SerializeField] EyesRotation eyes;
    [SerializeField] Transform[] waypoints; 
    public float stepTime = 1;
    public ChasingPath pathBuilder;
    public GameObject testObject;

    bool aiMovePatrol, aiMove , aiUpdate = false;
    bool attack = false;
    int lastwaypoint = 0;
    Vector3 lastTilePos;

    UnityEvent PatrolAgain = new UnityEvent();

    States states;

    IEnumerator UpdateView()
    {

        while (true)
        {
            yield return new WaitForSeconds(stepTime);
            if (eyes.player != null && states !=States.Melee)
            {
                states = States.Chase;
            }
            if(eyes.player == null)
            {
                pathBuilder.GetTileFromVector(lastTilePos).Walkable = true;
                states = States.Patrol;
            }
            switch (states)
            {
                case States.Patrol:
                    if (!aiMovePatrol)
                    {
                        PatrollingSwitch();
                    }
                    break;
                case States.Chase:
                    if (!aiMove)
                    {
                        MoveToPlayer();
                    }

                    break;
                case States.Melee:
                    pathBuilder.GetTileFromVector(transform.position).Walkable = false;

                    break;
            }
        }
    }

    private void Start()
    {
        //PatrolAgain.AddListener(PatrollingSwitch);
        lastTilePos = transform.position;
        states = States.Patrol;
        StartCoroutine(UpdateView());
    }

    // These two functions take signal from trigger attached to enemy
    public void StartMeleeAttack(Collider playerCollider)
    {
        if (playerCollider.tag == "Player")
        {
            states = States.Melee;
        }
        //print(playerCollider+"   "+ attack);
    }

    public void StopMeleeAttack(Collider playerCollider)
    {
        if (playerCollider.tag == "Player")
        {
            pathBuilder.GetTileFromVector(transform.position).Walkable = true;
            states = States.Patrol;
        }
    }

    private void Update()
    {
        //RaycastforPlayer();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (eyes.player == null)
            {
                print("trigger");
                player = other.transform;
                eyes.player = player;
            }
        }
        else
        {
            print("No target");
            eyes.player = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            eyes.player = null;
        }
    }

    private void RaycastforPlayer()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, eyes.transform.position.y / 2, eyes.transform.forward, out hit, 6))
        {
            if (hit.collider == null) { eyes.player = null; }

            if (hit.collider.tag == "Player")
            {
                if(eyes.player == null)
                {
                    player = hit.collider.transform;
                    eyes.player = player;
                }
            }
            else
            {
                if (eyes.player != null)
                {
                    eyes.player = null;
                    print("does update work " + aiUpdate + " and patrol is " + aiMovePatrol);
                }
            }
        }
    }

    private void OnDestroy()
    {
        PatrolAgain.RemoveAllListeners();
    }

    public void PatrollingSwitch()
    {
        if (lastwaypoint == waypoints.Length - 1) lastwaypoint = 0;
        else lastwaypoint++; 

        List<Vector3> path = pathBuilder.GetNewPath(transform.position, waypoints[lastwaypoint].position);
        if (path[0] == transform.position) pathBuilder.GetTileFromVector(lastTilePos).Walkable = true;
        GetNewPathPatrol (path);
    }

    public void MoveToPlayer()
    {
        //StopAllCoroutines();

        lastPlayerPosition = player.position;
        print("enemy at "+ pathBuilder.VectorToVectorInt( gameObject.transform.position) + "-"+ transform.position);
        List<Vector3> path = pathBuilder.GetNewPath(transform.position, player.position);
        print("first cell at "+ pathBuilder.VectorToVectorInt(path[0]));
        //print("route long" + path.Count);
        if (path[0] == transform.position) pathBuilder.GetTileFromVector(lastTilePos).Walkable = true;
        GetNewPath(path);
       
    }

    public void GetNewPath(List<Vector3> path)
    {
        StartCoroutine(Move(path));
        //StartCoroutine(UpdateView());
        //aiMovePatrol = false;
    }    
    
    public void GetNewPathPatrol(List<Vector3> path)
    {

        print("start patroling");
        StartCoroutine(MovePatrol(path));
    }

    IEnumerator Move(List<Vector3> path)
    {
        lastTilePos = transform.position;
        if (path.Count != 0)
        {
            lastTilePos = path[0];
        }
        
        while (path.Count > 0 && !attack)
        {
            aiMove = true;
            this.transform.LookAt(new Vector3(path[0].x, transform.position.y, path[0].z));
            yield return new WaitForSeconds(stepTime);
            if (path.Count > 1) 
            {
                // check if next node is not busy 
                if (pathBuilder.GetTileFromVector(path[0]).Walkable)
                {
                    pathBuilder.GetTileFromVector(lastTilePos).Walkable = true;
                    transform.position = new Vector3(path[0].x, transform.position.y, path[0].z);
                    pathBuilder.GetTileFromVector(path[0]).Walkable = false;
                }
                else
                {
                    pathBuilder.GetTileFromVector(lastTilePos).Walkable = true;
                    path.Clear(); 
                }
               
            }
            // check if next node is not busy 
            if (!CheckStopFactor()) { pathBuilder.GetTileFromVector(lastTilePos).Walkable = true; path.Clear(); }
            if (path.Count != 0)
            {
                lastTilePos = path[0];
                path.RemoveAt(0);
            }
        }
        aiMove = false;
    }

    IEnumerator MovePatrol(List<Vector3> path)
    {
        aiMovePatrol = true;
        while (path.Count > 0 && !attack)
        {
            
            this.transform.LookAt(new Vector3(path[0].x, transform.position.y, path[0].z));
            transform.position = new Vector3(path[0].x, transform.position.y, path[0].z);
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
            if (states != States.Patrol) { path.Clear(); aiMovePatrol = false; break; }
        }
        aiMovePatrol = false;
        //if (!attack) PatrolAgain.Invoke();
    }
     
    bool CheckStopFactor()
    {
        if (states != States.Chase) return false;
        if (lastPlayerPosition != player.position) return false;
        if (attack) return false;
        //if (eyes.player != null) { aiMove = false; return false; }
        return true;
    }

}

