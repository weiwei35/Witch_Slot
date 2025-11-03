using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding
{
    private int Move_Straight_Cost = 10;
    private int Move_Diagonal_Cost = 14;
    
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height,Vector3 start)
    {
        grid = new Grid<PathNode>(width, height,1,start, (Grid<PathNode> g,int x,int y) => new PathNode(g,x,y) );
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        
        openList = new List<PathNode>{startNode};
        closedList = new List<PathNode>();

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                PathNode node = grid.GetGridObject(i, j);
                node.gCost = int.MaxValue;
                node.CalculateFCost();
                node.cameFrom = null;
            }
        }
        
        startNode.gCost = 0;
        startNode.fCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //final node
                return CalculatePath(endNode);
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in GetNeighbours(currentNode))
            {
                if(closedList.Contains(neighbour)) continue;
                if (!neighbour.isWalkable)
                {
                    closedList.Add(neighbour);
                    continue;
                }
                int newGCost = currentNode.gCost + CalculateDistance(currentNode, neighbour);
                if (newGCost < neighbour.gCost)
                {
                    neighbour.cameFrom = currentNode;
                    neighbour.gCost = newGCost;
                    neighbour.hCost = CalculateDistance(neighbour, endNode);
                    neighbour.CalculateFCost();
                }

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }
        //out while
        return null;
    }

    private List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();
        if (node.GetX() - 1 >= 0)
        {
            //left
            neighbours.Add(grid.GetGridObject(node.GetX() - 1, node.GetY()));
        }
        if (node.GetX() + 1 < grid.GetWidth())
        {
            //right
            neighbours.Add(grid.GetGridObject(node.GetX() + 1, node.GetY()));
        }
        if (node.GetY() - 1 >= 0)
        {
            //down
            neighbours.Add(grid.GetGridObject(node.GetX(), node.GetY() - 1));
        }
        if (node.GetY() + 1 < grid.GetHeight())
        {
            //top
            neighbours.Add(grid.GetGridObject(node.GetX(), node.GetY() + 1));
        }
        return neighbours;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFrom != null)
        {
            path.Add(currentNode.cameFrom);
            currentNode = currentNode.cameFrom;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(PathNode a, PathNode b)
    {
        if (a != null&& b != null)
        {
            int xDistance = Mathf.Abs(a.GetX() - b.GetX());
            int yDistance = Mathf.Abs(a.GetY() - b.GetY());
            int result = Mathf.Abs(xDistance - yDistance);
            return Move_Diagonal_Cost * Mathf.Min(xDistance, yDistance) + Move_Straight_Cost * result;
        }
        return -1;
    }

    private PathNode GetLowestFCostNode(List<PathNode> nodes)
    {
        PathNode lowestFCostNode = nodes[0];
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodes[i];
            }
        }
        return lowestFCostNode;
    }
}
