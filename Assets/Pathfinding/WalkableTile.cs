using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableTile : MonoBehaviour
{
    public Vector3Int tilePosition;
    public int distanceToTarget = 0;
    public int distanceFromStart = 0;
    public bool isWalkable = true;
    public WalkableTile parent;
    public bool isBusy = false;

    public int DistanceBetweenNodes(WalkableTile start, WalkableTile end, int COST_DIAGONAL,int COST_STRAIGHT)
    {
        int distX = Mathf.Abs(start.tilePosition.x - end.tilePosition.x);
        int distY = Mathf.Abs(start.tilePosition.y - end.tilePosition.y);
        int delta = Mathf.Abs(distX - distY);

        if (distX > distY)
        {
            return COST_DIAGONAL * distY + COST_STRAIGHT * delta;
        }
        return COST_DIAGONAL * distX + COST_STRAIGHT * delta;
    }

    public int DistancesCost()
    {
        return distanceToTarget + distanceFromStart;
    }

    public WalkableTile(Vector3Int tilePosition, bool isWalkable)
    {
        this.tilePosition = tilePosition;
        this.isWalkable = isWalkable;
    }

    public List<WalkableTile> neighbourNodes(WalkableTile start, WalkableTile end, Dictionary<Vector3Int, WalkableTile> listOFNodes, int constD, int constS)
    {
        List<WalkableTile> nodes = new List<WalkableTile>();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue; //skip central tile

                Vector3Int nPlace = new Vector3Int(this.tilePosition.x + x, this.tilePosition.y + y);
                if (!listOFNodes.ContainsKey(nPlace)) continue;
                if (!listOFNodes[nPlace].isWalkable) continue;
                    if (x == -1 && y == 0)
                    {
                        listOFNodes[nPlace].distanceToTarget = DistanceBetweenNodes(listOFNodes[nPlace], end, constD,constS);
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




