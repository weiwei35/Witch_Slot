using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridObjectChangeedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangeedEventArgs: EventArgs
    {
        public int x;
        public int y;
    }
    
    private int width;//表格宽度
    private int height;//表格高度
    private float cellSize;
    private Vector3 origin;
    private TGridObject[,] gridArray;//所有格子列表
    private TextMeshPro[,] debugText;

    bool isDebug = false;
    public Grid(int width, int height, float cellSize, Vector3 origin,Func<Grid<TGridObject>,int,int,TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        
        gridArray = new TGridObject[width, height];
        debugText = new TextMeshPro[width, height];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = createGridObject(this,i,j);
            }
        }

        if (isDebug)
        {
            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    //遍历所有格子
                    debugText[i,j] = UtilsClass.CreateWorldText(gridArray[i,j]?.ToString(),null,GetWorldPosition(i,j)+Vector3.one*cellSize/2,3,Color.white,TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(i,j), GetWorldPosition(i,j+1), Color.white,100f);
                    Debug.DrawLine(GetWorldPosition(i,j),GetWorldPosition(i+1,j), Color.white,100f);
                }
            }
            
            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height), Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width,height),GetWorldPosition(width,0), Color.white,100f);
        }
        
        OnGridObjectChanged += (object sender, OnGridObjectChangeedEventArgs e) =>
        {
            debugText[e.x,e.y].text = gridArray[e.x,e.y]?.ToString();
        };
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y)*cellSize + origin;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos-origin).x / cellSize);
        y = Mathf.FloorToInt((worldPos-origin).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if(x < 0 || y < 0 || x >= width || y >= height) return;
        gridArray[x,y] = value;
        debugText[x,y].text = gridArray[x,y].ToString();
        if(OnGridObjectChanged!=null) OnGridObjectChanged(this, new OnGridObjectChangeedEventArgs { x = x, y = y });
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if(OnGridObjectChanged!=null) OnGridObjectChanged(this, new OnGridObjectChangeedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPos, TGridObject value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x,y];
        }
        return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetGridObject(x, y);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public TGridObject[,] GetGridArray()
    {
        return gridArray;
    }
}
