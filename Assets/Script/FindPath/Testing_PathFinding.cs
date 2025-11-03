using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Testing_PathFinding : MonoBehaviour
{
    public static Testing_PathFinding instance;
    
    public LayerMask layerMask;
    public Pathfinding pathfinding;

    private void Awake()
    {
        instance = this;
        
        SetGridSystem(20,20,new Vector3(-10f,-10f,0));
    }

    public void SetGridSystem(int width, int height,Vector3 origin)
    {
        transform.position = origin;
        pathfinding = new Pathfinding(width, height,origin);
    }
    private bool CheckCollisionAtPoint(Vector2 point, LayerMask collisionLayers)
    {
        // 使用Layer过滤，只检测指定层的碰撞体
        Collider2D collider = Physics2D.OverlapPoint(point, collisionLayers);
    
        return collider != null;
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        SetMapCollider();
        pathfinding.GetGrid().GetXY(start, out int startX, out int startY);
        pathfinding.GetGrid().GetXY(end, out int x, out int y);
        // Debug.Log("起始："+startX+" "+startY);
        // Debug.Log("结束："+x+" "+y);
        List<PathNode> path = new List<PathNode>();
        path = pathfinding.FindPath(startX, startY, x,y);
        List<Vector3> result = new List<Vector3>();
        
        if (path != null)
        {
            foreach (var node in path)
            {
                float posX = node.GetX()*0.8f+0.4f;
                float posY = node.GetY()*0.8f+0.4f;
                Vector3 pos = new Vector3(posX, posY,0) + transform.position;
                // result.Add(new Vector3(node.GetX()+0.4f,node.GetY()+0.4f)+transform.position);
                result.Add(pos);
            }
            for (int i = 0; i < path.Count-1; i++)
            {
                float posX1 = path[i].GetX()*0.8f+0.4f;
                float posY1 = path[i].GetY()*0.8f+0.4f;
                Vector3 pos1 = new Vector3(posX1, posY1,0) + transform.position;
                float posX2 = path[i+1].GetX()*0.8f+0.4f;
                float posY2 = path[i+1].GetY()*0.8f+0.4f;
                Vector3 pos2 = new Vector3(posX2, posY2,0) + transform.position;
                // Debug.DrawLine(new Vector3(path[i].GetX()+0.4f,path[i].GetY()+0.4f)+transform.position,new Vector3(path[i+1].GetX()+0.4f,path[i+1].GetY()+0.4f)+transform.position, Color.green,10);

                Debug.DrawLine(pos1,pos2, Color.green,10);
            }
        }
        return result;
    }

    private void SetMapCollider()
    {
        PathNode[,] gridArray = pathfinding.GetGrid().GetGridArray();
        foreach (var node in gridArray)
        {
            float x = node.GetX()*0.8f+0.4f;
            float y = node.GetY()*0.8f+0.4f;
            Vector3 pos = new Vector3(x, y,0) + transform.position;
            // Debug.Log(pos);
            if (CheckCollisionAtPoint(pos, layerMask))
            {
                node.isWalkable = false;
            }
            else
            {
                node.isWalkable = true;
            }
        }
    }
}
