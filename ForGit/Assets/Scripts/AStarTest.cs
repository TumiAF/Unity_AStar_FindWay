using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    public int beginX = 0; //左上角第一个立方体
    public int beginY = 0;

    public int offsetX = 2; //立方体的偏移
    public int offsetY = 2;

    //地图宽高
    public int mapW = 5;
    public int mapH = 5;
    

    private Dictionary<string,GameObject> cubesDic = new Dictionary<string, GameObject>() ;

    //开始点
    private Vector2 beginPos = Vector2.right * -1;
    void Start()
    {
        AStarManager.Instance.InitMapInfo(mapW,mapH); //初始化地图信息

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                //创建立方体
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX+i*offsetX,beginY+j*offsetY,0);
                obj.name = i + "_" + j;

                cubesDic.Add(obj.name,obj); //存入字典，名字和物体对应

                AStarNode node = AStarManager.Instance.nodes[i,j];
                if(node.type == NodeType.Stop) //如果是阻挡
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.red; //变为红色
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //检测鼠标左键点击
        if(Input.GetMouseButtonDown(0))
        {
            //射线检测
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray,out info,1000))
            {
                //记录开始点
                if(beginPos == Vector2.right * -1)
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    beginPos = new Vector2(int.Parse(strs[0]),int.Parse(strs[1])); //获得beginPos坐标
                    info.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else //有起点了，再点个终点
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]),int.Parse(strs[1]));

                    //开始寻路
                    List<AStarNode> path =  AStarManager.Instance.FindPath(beginPos,endPos);
                    if(path!=null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {   
                            cubesDic[path[i].x+"_"+path[i].y].GetComponent<MeshRenderer>().material.color = Color.green;
                        }
                    }
                }
                
                
            }
        }
    }
}
