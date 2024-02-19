using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerTileMove : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public Transform cameraTarget;
    public Tilemap moveTilemap;
    Vector3Int startposition;
    Vector3Int currentposition;
    OnBlockPlacement currentWallBlock;
    CardinalDirections currentforwardDirection;
    Dictionary<Vector3Int, OnBlockPlacement> wallsAccess = new Dictionary<Vector3Int, OnBlockPlacement>();
    public  PartyManager party;
    public UnityEvent noWay, stepSound, turnAround, portalTransfer;


    void Start()
    {
        
        if (party.GetPartyState() == PartyState.HeroMenu) return;
        startposition = moveTilemap.WorldToCell(transform.position);
        var v = moveTilemap.GetCellCenterWorld(startposition);
        transform.position = new Vector3(v.x, transform.position.y, v.z);
        currentposition = startposition;
        RotateToCardinalLocation();
        var walls = moveTilemap.GetComponentsInChildren<OnBlockPlacement>();

        foreach (OnBlockPlacement w in walls)
        {
            if (!wallsAccess.ContainsKey(w.position))
            wallsAccess.Add(w.position, w);
        }
        currentWallBlock = wallsAccess[currentposition];
        currentWallBlock.isVisited = true;

        party.SetPartyState(PartyState.Explore);
    }

    void RotateToCardinalLocation()
    {
        currentforwardDirection = CardinalDirections.North;
        if (transform.rotation.eulerAngles.y != 0)
        {
            float YAngle = transform.rotation.eulerAngles.y%360;


            if (YAngle > 0 && YAngle < 45)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            }
            if (YAngle > 45 && YAngle < 135)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                TurnRight();
                return;
            }
            if (YAngle > 135 && YAngle < 225)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                TurnRight(); TurnRight();
                return;
            }
            if (YAngle > 225 && YAngle < 325)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                TurnLeft();
                return;
            }
        }
    }

    void Update()
    {

        if (party.GetPartyState() != PartyState.Explore) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBackward();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StrafeRight();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StrafeLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TurnRight();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TurnLeft();
        }
    }


    public void MoveForward()
    {
        if (!currentWallBlock.IfWallOpened(currentforwardDirection)) { noWay.Invoke();  return; }

        Vector3 v = CardinalDir.GetNewPoint(currentforwardDirection, currentposition, moveTilemap);
        if (!wallsAccess[moveTilemap.WorldToCell(v)].Walkable) return;

        if (wallsAccess[moveTilemap.WorldToCell(v)].isPortal) {
            transform.position = new Vector3(v.x, transform.position.y, v.z);
            StartCoroutine(WaitForSomeSeconds(0.5f, v));
        }
        else
        {
            transform.position = new Vector3(v.x, transform.position.y, v.z);
            currentposition = moveTilemap.WorldToCell(transform.position);
            currentWallBlock = wallsAccess[currentposition];
            if (!currentWallBlock.isVisited) currentWallBlock.isVisited = true;
            stepSound.Invoke();
        }
    }

    private IEnumerator UpdateCameraFrameLater()
    {
        yield return null;
        CinemachineFramingTransposer transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer == null) transposer = vcam.gameObject.AddComponent<CinemachineFramingTransposer>();
        vcam.Follow = cameraTarget;
        transposer.ForceCameraPosition(cameraTarget.transform.position, cameraTarget.rotation);
        vcam.enabled = true;
    }

    IEnumerator WaitForSomeSeconds(float sec, Vector3 v)
    {
        yield return new WaitForSeconds(sec);
        v = moveTilemap.GetCellCenterWorld(wallsAccess[moveTilemap.WorldToCell(v)].GetPortalDestination());
        transform.position = new Vector3(v.x, transform.position.y, v.z);
        currentposition = moveTilemap.WorldToCell(transform.position);
        currentWallBlock = wallsAccess[currentposition];
        //stepSound.Invoke();

        vcam.Follow = null;
        vcam.enabled = false;
        vcam.gameObject.transform.position = cameraTarget.position;
        vcam.gameObject.transform.rotation = cameraTarget.rotation;
        vcam.ForceCameraPosition(cameraTarget.transform.position, cameraTarget.rotation);
        StartCoroutine(UpdateCameraFrameLater());
    }

    public void MoveBackward()
    {
        if (!currentWallBlock.IfWallOpened(CardinalDir.GetOpposite(currentforwardDirection))) return;
        var v = CardinalDir.GetNewPoint(CardinalDir.GetOpposite(currentforwardDirection), currentposition, moveTilemap);
        if (!wallsAccess[moveTilemap.WorldToCell(v)].Walkable) return;
        transform.position = new Vector3(v.x, transform.position.y, v.z);
        currentposition = moveTilemap.WorldToCell(transform.position);
        currentWallBlock = wallsAccess[currentposition];
        stepSound.Invoke();
    }

    public void StrafeRight()
    {
        if (!currentWallBlock.IfWallOpened(CardinalDir.GetRightDir(currentforwardDirection))) return;
        var v = CardinalDir.GetNewPoint(CardinalDir.GetRightDir(currentforwardDirection), currentposition, moveTilemap);
        if (!wallsAccess[moveTilemap.WorldToCell(v)].Walkable) return;
        transform.position = new Vector3(v.x, transform.position.y, v.z);
        currentposition = moveTilemap.WorldToCell(transform.position);
        currentWallBlock = wallsAccess[currentposition];
        stepSound.Invoke();
    }

    public void StrafeLeft()
    {
        if (!currentWallBlock.IfWallOpened(CardinalDir.GetOpposite(CardinalDir.GetRightDir(currentforwardDirection)))) return;
        var v = CardinalDir.GetNewPoint(CardinalDir.GetOpposite(CardinalDir.GetRightDir(currentforwardDirection)), currentposition, moveTilemap);
        if (!wallsAccess[moveTilemap.WorldToCell(v)].Walkable) return;
        transform.position = new Vector3(v.x, transform.position.y, v.z);
        currentposition = moveTilemap.WorldToCell(transform.position);
        currentWallBlock = wallsAccess[currentposition];
        stepSound.Invoke();
    }

    public void TurnRight()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0);
        currentforwardDirection = CardinalDir.SetDirectionRight(currentforwardDirection);
        turnAround.Invoke();
    }
    public void TurnLeft()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
        currentforwardDirection = CardinalDir.SetDirectionLeft(currentforwardDirection);
        turnAround.Invoke();
    }


    public Vector2 GetcurrentPosition()
    {
        return new Vector2(currentposition.x, currentposition.y);
    }

    public CardinalDirections GetCurrentDirection()
    {
        return  currentforwardDirection;
    }

}
