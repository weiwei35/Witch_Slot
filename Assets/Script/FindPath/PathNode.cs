using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    public int gCost;
    public int hCost;
    public int fCost;
    
    public bool isWalkable;
    public PathNode cameFrom;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public int GetX()
    {
        if(x >= 0 && x < grid.GetWidth())
            return x;
        else return -1;
    }

    public int GetY()
    {
        if(y >= 0 && x < grid.GetWidth())
            return y;
        else return -1;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

}
