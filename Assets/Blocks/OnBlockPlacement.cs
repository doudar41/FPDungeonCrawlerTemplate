using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

/// <summary>
/// This script should be placed on prefab used as tilebrush for tilemap. 
/// This is a cube of any size build with 6 colliders.
/// walls, ceilling and floor. 
/// It used by player controller to move around map. Also it can be used to make a map instead of using up camera.
/// It also is used for pathfinding and portals.
/// This script also contains cardinal direction class, which is used to set world directions for anything. 
/// </summary>



public class OnBlockPlacement : MonoBehaviour
{

    public TextMeshPro coordinates;
    public GameObject[] walls = new GameObject[4];
    //Walls should be set inside a prefab by dragging gameobjects with walls colliders
    //In this order North, East, South, West where North is wall facing from cube to Z (blue) axis.

    // Pathfinding variables
    public Vector3Int position;
    public int distanceToTarget = 0;
    public int distanceFromStart = 0;
    public OnBlockPlacement blockParent; //temporary parent for pathfinding
    const int COST_STRAIGHT = 10;
    const int COST_DIAGONAL = 14;

    public bool Walkable = true;
    public bool isPortal;
    public bool isVisited = false;
    [SerializeField] OnBlockPlacement portalDestination;

    private void Start()
    {
        coordinates.gameObject.SetActive(false);
    }

    public int DistancesCost()
    {
        return distanceToTarget + distanceFromStart;
    }


    // This functions are used for tilemap building in editor 
    public void CheckGridForGameObject(Tilemap tilemap, Vector3Int position)
    {
        var blocks = transform.parent.GetComponentsInChildren<Transform>();
        CheckWallsForNeibors(tilemap, CardinalDirections.East, position, 1, 0, blocks);
        CheckWallsForNeibors(tilemap, CardinalDirections.West, position, -1, 0, blocks);
        CheckWallsForNeibors(tilemap, CardinalDirections.North, position, 0, 1, blocks);
        CheckWallsForNeibors(tilemap, CardinalDirections.South, position, 0, -1, blocks);
    }

    public void CheckWallsForNeibors(Tilemap tilemap,
                                    CardinalDirections wallIndex,
                                    Vector3Int position,
                                    int shiftBlockX,
                                    int shiftBlockY,
                                    Transform[] blocks)
    {
        //it works good for block with flat wall's textures, it it needs 3D walls logic should be more complicated

        Vector3 BlockWorldCoordinate = tilemap.GetCellCenterWorld(new Vector3Int(position.x + shiftBlockX, position.y + shiftBlockY, position.z));
        foreach (Transform p in blocks)
        {
            if (p.position == new Vector3(BlockWorldCoordinate.x, p.position.y, BlockWorldCoordinate.z))
            {
                var neighbour = p.gameObject.GetComponent<OnBlockPlacement>();

                walls[(int)wallIndex].SetActive(false);
                if (neighbour != null)
                    neighbour.walls[(int)CardinalDir.GetOpposite(wallIndex)].SetActive(false);
                //Debug.Log("Disable " + walls[(int)wallIndex].name);
            }
        }
    }
    //This function called by player controller to check if it's possibly walkable 
    public bool IfWallOpened(CardinalDirections dir)
    {
        bool access = true;
        switch (dir)
        {
            case CardinalDirections.East:
                if (walls[1].activeSelf) access = false;
                break;
            case CardinalDirections.South:
                if (walls[2].activeSelf) access = false;
                break;
            case CardinalDirections.West:
                if (walls[3].activeSelf) access = false;
                break;
            case CardinalDirections.North:
                if (walls[0].activeSelf) access = false;
                break;
        }
        return access;
    }

    public void SetWalkable(bool walk)
    {
        Walkable = walk;
    }

    public void EnablePortal(bool portal)
    {
        isPortal = portal;
    }

    public Vector3Int GetPortalDestination()
    {
        return portalDestination.position;
    }

    public void CoordinatesToText()
    {
        coordinates.text = position.ToString();
    }

    /// <summary>
    /// This is pathfinder function returning costs of neighbor nodes
    /// </summary>


    public int DistanceBetweenNodes(OnBlockPlacement start, OnBlockPlacement end, int COST_DIAGONAL, int COST_STRAIGHT)
    {
        int distX = Mathf.Abs(start.position.x - end.position.x);
        int distY = Mathf.Abs(start.position.y - end.position.y);
        int delta = Mathf.Abs(distX - distY);

        if (distX > distY)
        {
            return COST_DIAGONAL * distY + COST_STRAIGHT * delta;
        }
        return COST_DIAGONAL * distX + COST_STRAIGHT * delta;
    }

    public List<OnBlockPlacement> neighbourNodes(OnBlockPlacement start, OnBlockPlacement end, Dictionary<Vector3Int, OnBlockPlacement> listOFNodes, int constD, int constS)
    {
        List<OnBlockPlacement> nodes = new List<OnBlockPlacement>();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue; //skip central tile

                Vector3Int nPlace = new Vector3Int(this.position.x + x, this.position.y + y);
                if (!listOFNodes.ContainsKey(nPlace)) continue;
                if (!listOFNodes[nPlace].Walkable) continue;
                if (x == -1 && y == 0)
                {
                    listOFNodes[nPlace].distanceToTarget = DistanceBetweenNodes(listOFNodes[nPlace], end, constD, constS);
                    nodes.Add(listOFNodes[nPlace]);
                }
                if (x == 0 && y == 1)
                {
                    listOFNodes[nPlace].distanceToTarget = DistanceBetweenNodes(listOFNodes[nPlace], end, constD, constS);
                    nodes.Add(listOFNodes[nPlace]);
                }
                if (x == 0 && y == -1)
                {
                    listOFNodes[nPlace].distanceToTarget = DistanceBetweenNodes(listOFNodes[nPlace], end, constD, constS);
                    nodes.Add(listOFNodes[nPlace]);
                }
                if (x == 1 && y == 0)
                {
                    listOFNodes[nPlace].distanceToTarget = DistanceBetweenNodes(listOFNodes[nPlace], end, constD, constS);
                    nodes.Add(listOFNodes[nPlace]);
                }

            }
        }
        return nodes;
    }
}




/// <summary>
/// Cardinal system with functions for player controller and pathfinding 
/// </summary>

[System.Serializable]
public enum CardinalDirections
{
    North,
    East,
    South,
    West
}

public static class CardinalDir
{
    public static CardinalDirections GetOpposite(CardinalDirections dir)
    {
        CardinalDirections dirOpposite = new CardinalDirections();
        switch (dir)
        {
            case CardinalDirections.East:
                dirOpposite = CardinalDirections.West;
                break;
            case CardinalDirections.South:
                dirOpposite = CardinalDirections.North;
                break;
            case CardinalDirections.West:
                dirOpposite = CardinalDirections.East;
                break;
            case CardinalDirections.North:
                dirOpposite = CardinalDirections.South;
                break;
        }
        return dirOpposite;
    }

    public static Vector3 GetNewPoint(CardinalDirections currentdir, Vector3Int startPosition, Tilemap tilemap)
    {
        //Debug.Log("forward");
        Vector3 v = new Vector3();
        Vector3Int dir;
        switch (currentdir)
        {
            case CardinalDirections.East:
                dir = new Vector3Int(startPosition.x + 1, startPosition.y);
                v = tilemap.GetCellCenterWorld(dir);
                break;
            case CardinalDirections.South:
                dir = new Vector3Int(startPosition.x, startPosition.y - 1);
                v = tilemap.GetCellCenterWorld(dir);
                break;
            case CardinalDirections.West:
                dir = new Vector3Int(startPosition.x - 1, startPosition.y);
                v = tilemap.GetCellCenterWorld(dir);
                break;
            case CardinalDirections.North:
                dir = new Vector3Int(startPosition.x, startPosition.y + 1);
                v = tilemap.GetCellCenterWorld(dir);
                break;
        }
        return v;
    }


    public static CardinalDirections GetRightDir(CardinalDirections dir)
    {
        CardinalDirections dirOpposite = new CardinalDirections();
        switch (dir)
        {
            case CardinalDirections.East:
                dirOpposite = CardinalDirections.South;
                break;
            case CardinalDirections.South:
                dirOpposite = CardinalDirections.West;
                break;
            case CardinalDirections.West:
                dirOpposite = CardinalDirections.North;
                break;
            case CardinalDirections.North:
                dirOpposite = CardinalDirections.East;
                break;
        }

        return dirOpposite;
    }


    public static CardinalDirections SetDirectionRight(CardinalDirections currentdir)
    {
        CardinalDirections dirRight = new CardinalDirections();

        switch (currentdir)
        {
            case CardinalDirections.East:

                dirRight = CardinalDirections.South;
                break;
            case CardinalDirections.South:

                dirRight = CardinalDirections.West;
                break;
            case CardinalDirections.West:

                dirRight = CardinalDirections.North;
                break;
            case CardinalDirections.North:
                dirRight = CardinalDirections.East;
                break;
        }
        return dirRight;
    }

    public static CardinalDirections SetDirectionLeft(CardinalDirections currentdir)
    {
        CardinalDirections dirRight = new CardinalDirections();

        switch (currentdir)
        {
            case CardinalDirections.East:

                dirRight = CardinalDirections.North;
                break;
            case CardinalDirections.South:

                dirRight = CardinalDirections.East;
                break;
            case CardinalDirections.West:

                dirRight = CardinalDirections.South;
                break;
            case CardinalDirections.North:
                dirRight = CardinalDirections.West;
                break;
        }
        return dirRight;
    }

    public static int DistanceBetweenNodes(OnBlockPlacement start, OnBlockPlacement end, int COST_DIAGONAL, int COST_STRAIGHT)
    {
        int distX = Mathf.Abs(start.position.x - end.position.x);
        int distY = Mathf.Abs(start.position.y - end.position.y);
        int delta = Mathf.Abs(distX - distY);

        if (distX > distY)
        {
            return COST_DIAGONAL * distY + COST_STRAIGHT * delta;
        }
        return COST_DIAGONAL * distX + COST_STRAIGHT * delta;
    }

}


