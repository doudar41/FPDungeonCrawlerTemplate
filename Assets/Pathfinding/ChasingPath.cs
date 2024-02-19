using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChasingPath : MonoBehaviour
{
    public Tilemap movementMap;
    const int COST_STRAIGHT = 10;
    const int COST_DIAGONAL = 14;
    

    Dictionary<Vector3Int, OnBlockPlacement> listOfNodes = new Dictionary<Vector3Int, OnBlockPlacement>();

    private void Start()
    {
        OnBlockPlacement[] tempNodes= movementMap.gameObject.GetComponentsInChildren<OnBlockPlacement>();
        foreach (OnBlockPlacement g in tempNodes)
        {
            g.GetComponent<OnBlockPlacement>().position = movementMap.WorldToCell(g.transform.position);
            listOfNodes.Add(g.GetComponent<OnBlockPlacement>().position, g.GetComponent<OnBlockPlacement>());
        }
        print(listOfNodes.Count);
    }

    public List<Vector3> GetNewPath(Vector3 start, Vector3 end)
    {
        return CalculatePath(movementMap.WorldToCell(start), movementMap.WorldToCell(end));
    }

    public List<Vector3> CalculatePath(Vector3Int startTile, Vector3Int endTile)
    {
        if (!listOfNodes.ContainsKey(endTile))
        {
            List<Vector3> path = new List<Vector3> { movementMap.GetCellCenterWorld(startTile) };
            return path;
        }
        if (startTile == endTile || !listOfNodes[endTile].Walkable)
        {
            List<Vector3> path = new List<Vector3> { movementMap.GetCellCenterWorld(startTile) };
            return path;
        }

        List<OnBlockPlacement> openedTiles = new List<OnBlockPlacement>();
        HashSet<OnBlockPlacement> closedTiles = new HashSet<OnBlockPlacement>();
        List<Vector3> tempPath = new List<Vector3>();

        OnBlockPlacement newStartTile = listOfNodes[startTile];
        OnBlockPlacement endNode = listOfNodes[endTile];

        newStartTile.distanceFromStart = 0;
        newStartTile.distanceToTarget = DistanceBetweenNodes(newStartTile, endNode);
        endNode.distanceFromStart = DistanceBetweenNodes(newStartTile, endNode);

        openedTiles.Add(newStartTile);

        while (openedTiles.Count > 0)
        {
            if (!listOfNodes[endTile].Walkable)
            {
                List<Vector3> path = new List<Vector3> { movementMap.GetCellCenterWorld(startTile) };
                return path;
            }
            OnBlockPlacement currentNode = openedTiles[0];
            for (int i = 1; i < openedTiles.Count; i++)
            {
                if (openedTiles[i].DistancesCost() < currentNode.DistancesCost() ||
                    openedTiles[i].DistancesCost() == currentNode.DistancesCost())
                {
                    if (openedTiles[i].distanceToTarget < currentNode.distanceToTarget)
                    {
                        currentNode = openedTiles[i];
                    }
                }
            }

            openedTiles.Remove(currentNode);
            closedTiles.Add(currentNode);

            if (currentNode.position == endNode.position)
            {
                //print("end of the line");
                List<Vector3> realPath = new List<Vector3>();

                OnBlockPlacement nextNode = listOfNodes[endTile];
                while (nextNode != listOfNodes[startTile])
                {
                    realPath.Add(movementMap.GetCellCenterWorld(nextNode.position));

                    if (nextNode.blockParent == null) { realPath.Reverse();  return realPath; }
                    nextNode = nextNode.blockParent;
                }
                realPath.Reverse();
                return realPath;
            }

            var nodeList = currentNode.neighbourNodes(newStartTile, endNode, listOfNodes, COST_DIAGONAL, COST_STRAIGHT);

            foreach (OnBlockPlacement n in nodeList)
            {
                if (closedTiles.Contains(n))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.distanceFromStart + DistanceBetweenNodes(currentNode, n);
                if (newCostToNeighbour < n.distanceFromStart || !openedTiles.Contains(n))
                {
                    n.distanceFromStart = newCostToNeighbour;
                    if (!openedTiles.Contains(n))
                    {
                        n.blockParent = currentNode;
                        openedTiles.Add(n);
                    }
                }
            }
        }
        List<Vector3> path_null = new List<Vector3> { movementMap.GetCellCenterWorld(startTile) };
        return path_null;
    }

    public Vector3Int VectorToVectorInt(Vector3 pos)
    {
        return movementMap.WorldToCell(pos);
    }

    public OnBlockPlacement GetTileFromVector(Vector3 pos)
    {

        Vector3Int coord = movementMap.WorldToCell(pos);
        return listOfNodes[coord];

    }

    public static int DistanceBetweenNodes(OnBlockPlacement start, OnBlockPlacement end)
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
