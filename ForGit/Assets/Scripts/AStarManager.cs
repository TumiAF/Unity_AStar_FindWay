using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager 
{
    #region 单例
    private static AStarManager instance; 
    public static AStarManager Instance{
        get{
            if(instance==null)
            instance = new AStarManager();
            return instance;
        }
    }
    #endregion

    //地图的宽高
    public int mapW,mapH;

    //地图相关所有的格子对象容器
    public AStarNode[,] nodes;

    private List<AStarNode>openList = new List<AStarNode>(); //开启列表
    private List<AStarNode>closeList = new List<AStarNode>(); //关闭列表


    //初始化地图数据
    public void InitMapInfo(int _w,int _h)//5*5
    {
        nodes = new AStarNode[_w,_h]; //初始化nodes数组（容器一共可以装的node数量

        this.mapW = _w; //地图宽高的初始化
        this.mapH = _h;

        //根据宽高，创建格子
        for(int i = 0;i<_w;i++)
        {
            for(int j = 0;j<_h;j++)
            {
                //随机阻挡的生成
                AStarNode node = new AStarNode(i,j,Random.Range(0,100)<20? NodeType.Stop:NodeType.Walk);
                nodes[i,j] = node; //把生成的格子放到数组中 
            }
        }
    }


    //寻路方法
    public List<AStarNode> FindPath(Vector2 _startPos,Vector2 _endPos)
    {
        //1、传入的两个点是否合法：
        //**********(1)边界判断 
        if(_startPos.x<0 || _startPos.x>=mapW || _startPos.y<0 || _startPos.y>=mapH
        || _endPos.x<0 || _startPos.x >=mapW || _endPos.y<0 || _endPos.y>=mapH)
        {
            Debug.Log("开始或结束点位于地图之外，寻路失败！");
            return null;
        }
        //**********(2)是否为阻挡点
        AStarNode start = nodes[(int)_startPos.x,(int)_startPos.y];
        AStarNode end = nodes[(int)_endPos.x,(int)_endPos.y];
        if(start.type==NodeType.Stop || end.type==NodeType.Stop)
        {
            Debug.Log("开始或结束点为阻挡点，寻路失败！");
            return null;
        }

        //2、清空列表
        openList.Clear();
        closeList.Clear();

        //3、初始化结点，把开始结点加入到closelist
        start.InitNode();
        closeList.Add(start);

        while(true)
        {
            //4、从起点开始，找周围的点，放入openlist中；
            //左上 x-1,y-1
            FindNearbyNodeToOpenList(start.x-1,start.y-1,1.4f,start,end);
            //上 x,y-1
            FindNearbyNodeToOpenList(start.x,start.y-1,1f,start,end);
            //右上 x+1,y-1
            FindNearbyNodeToOpenList(start.x+1,start.y-1,1.4f,start,end);
            //左 x-1,y
            FindNearbyNodeToOpenList(start.x-1,start.y,1f,start,end);
            //右 x+1,y
            FindNearbyNodeToOpenList(start.x+1,start.y,1f,start,end);
            //左下 x-1,y+1
            FindNearbyNodeToOpenList(start.x-1,start.y+1,1.4f,start,end);
            //下 x,y+1
            FindNearbyNodeToOpenList(start.x,start.y+1,1f,start,end);
            //右下 x+1,y+1
            FindNearbyNodeToOpenList(start.x+1,start.y+1,1.4f,start,end);

            //死路判断
            if(openList.Count<=0)
            {
                Debug.Log("死路");
                return null;
            }

            //5、选出开启列表中寻路消耗最小的点
            openList.Sort(SortOpenList); //根据结点的f的值进行排序

            //6、放入closelist，在openlist中删除
            closeList.Add(openList[0]);

            start = openList[0]; //找到的这个点，作为新的起点，用作下一次寻路
            openList.RemoveAt(0);

            //7、如果已经是终点，返回路径；如果不是，继续寻路
            if(start==end)
            {
                //找到了，返回路径
                List<AStarNode> path = new List<AStarNode>();
                path.Add(end);
                while(end.exploredFrom!=null) //回溯路径
                {
                    path.Add(end.exploredFrom);
                    end = end.exploredFrom;
                }
                path.Reverse();
                return path;
            }
        }
        

        
    }
    //排序函数
    private int SortOpenList(AStarNode a,AStarNode b)
    {
        if(a.f>=b.f)
        return 1;
        else return -1;
    }

    //寻找周围的点，加入到openlist中
    private void FindNearbyNodeToOpenList(int _x,int _y,float _g,AStarNode _exploredFrom,AStarNode _end)
    {
        //边界判断 
        if(_x<0||_x>=mapW||_y<0||_y>=mapH) return;

        AStarNode node = nodes[_x,_y];

        //空 || 为阻挡 || 已经在开启或关闭列表中
        if(node==null || node.type==NodeType.Stop || closeList.Contains(node) || openList.Contains(node))
        {
            return;
        }

        node.exploredFrom = _exploredFrom; //父结点（从哪来的）赋值

        //计算f = g+h 值；
        //我离起点的距离 = 我父结点离起点的距离 + 我离我父对象的距离
        node.g = _exploredFrom.g + _g; 
        //曼哈顿距离 x+y
        node.h = Mathf.Abs(_end.x - node.x) + Mathf.Abs(_end.y - node.y);
        node.f = node.g+node.h;

        //加入openlist
        openList.Add(node);

    }
}
