using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


public struct Grid
{
    public long index;       //索引
    public Vector2 pos;      //地图位置
    public bool walkable;    //标记是否可走
}
public class AstarNode
{
    public AstarNode pNode;
    Grid grid;
    public int F;
    public int G;
    public int H;
    public AstarNode(Grid g)
    {
        this.grid = g;
    }
}
public class DCaculate
{
    List<AstarNode> openList;
    List<AstarNode> closeList;

    public void StartCaculate(Grid spos, Grid epos)
    {
        openList.Add(new AstarNode(spos));  //把初始结点加入开启列表

    }
}
public static class GridHelper
{
    static int row;       //可走网格总行
    static int column;    //可走网格总列
    static int hspan;     //横竖网格间距
    static int xspan;     //对角网格间距
    /// <summary>
    /// 初始化网格信息
    /// </summary>
    /// <returns></returns>
    public static Grid[,] InitMap()
    {
        return null;
    }
    /// <summary>
    /// 计算当前网格在
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    public static int[] CaculateFGH(Grid s, Grid e, Grid now)
    {
        return null;
    }
}
