using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType{ //格子类型
     Walk,Stop
}
public class AStarNode 
{
    [Header("寻路消耗公式")]
    public float f; //寻路消耗
    public float g; //离起点的距离
    public float h; //离终点的距离

    [Header("格子坐标")]
    public int x;
    public int y;


    public NodeType type; //格子类型

    public AStarNode exploredFrom;//从哪个结点来的 

    //带参构造
    public AStarNode(int _x,int _y,NodeType _type) 
    {
        this.x = _x;
        this.y = _y;
        this.type =_type;
    }

    public void InitNode() //初始化结点
    {
        this.f = 0;
        this.g = 0;
        this.h = 0;
        this.exploredFrom = null;
    }
}

