using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.UI.Image;


/// <summary>
/// 所有格子管理
/// </summary>
public class GridManager : MonoBehaviour
{


    /// <summary>
    /// 所有格子父节点
    /// </summary>
    private Transform allGridRoot;

    /// <summary>
    /// 列数
    /// </summary>
    public int Column;
    /// <summary>
    /// 行数
    /// </summary>
    public int Row;

    /// <summary>
    /// 格子宽度
    /// </summary>
    public float gridWidth;
    /// <summary>
    /// 格子高度
    /// </summary>
    public float gridHeight;

    /// <summary>
    /// 全部的方块
    /// </summary>
    private Tile[,] tiles;


    private void Awake()
    {
        allGridRoot = transform;
        allGridPrefab =new Dictionary<GridType, GameObject> ();
        //初始化
        AddPrefab(GridType.Empty);
        AddPrefab(GridType.Green);
        AddPrefab(GridType.Red);
        AddPrefab(GridType.Yellow);
        AddPrefab(GridType.Blue);
        AddPrefab(GridType.Orange);
    }

    private void Start()
    {
        tiles=new Tile[Row, Column];

        CreatAllEmptyGrid();

        AlignGridToBottomCenter();

        CreateCell();

        CheckAllMatchGrid();
    }

    private float tileSize = 1.024f;

    /// <summary>
    /// 创建所有空格子 也就是背景格
    /// </summary>
    public void CreatAllEmptyGrid()
    {
        GameObject obj;
        for (int i = 0; i < Row; i++) 
        {
            for (int j = 0; j < Column; j++)
            {
                obj = CreatGrid(GridType.Empty);
                obj.transform.localPosition = new Vector3(i * tileSize, -j * tileSize, 0);
                //obj.name = string.Format($"Grid_{i}_{j}");
            }
        }
    }

    /// <summary>
    /// 创建游戏物品 在格子背景上的
    /// </summary>
    public void CreateCell()
    {
        GameObject obj;
        GridType curGridType;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                curGridType = (GridType)GetRandomTile();
                obj = CreatGrid(curGridType);
                Tile tile=obj.AddComponent<Tile>();
                tile.Init(i, j, curGridType);
                obj.transform.localPosition = new Vector3(i * tileSize, -j * tileSize, 0);
                //obj.name = string.Format($"Grid_{i}_{j}");
                tiles[i, j] = tile;
            }
        }


    }

    /// <summary>
    /// 检测所有匹配格子
    /// </summary>
    public void CheckAllMatchGrid()
    {
        List<Tile> matchList=new List<Tile>();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                 matchList = MatchCalculate(tiles[i,j],i,j);

                //Debug.LogError($" 可消除的 x:{matchList[t].xIndex} y:{matchList[t].yIndex} ");
            }
        }
        //if (matchList != null || matchList.Count>0) {
        //    for (int t = 0; t < matchList.Count; t++)
        //    {
        //        Debug.LogError($" 可消除的 x:{matchList[t].xIndex} y:{matchList[t].yIndex} ");
        //    }
        //}
        
    }

    private List<Tile> MatchCalculate(Tile tile,int newX,int newY)
    {
        GridType curGridType = tile.gridType;
        List<Tile> matchRow = new List<Tile>();
        List<Tile> matchLine = new List<Tile>();
        List<Tile> finishedMatching = new List<Tile>();

        //行匹配
        matchRow.Add(tile);

        //i=0代表往左，i=1代表往右
        for (int i = 0; i <=1; i++) 
        {
            for (int xDistance = 1; xDistance < Column; xDistance++) 
            {
                int x;
                if (i == 0)
                {
                    x = newX - xDistance;
                }
                else
                {
                    x = newX + xDistance;
                }
                if (x < 0 || x >= Column)
                {
                    break;
                }
                if (tiles[x,newY].gridType == curGridType)
                {
                    matchRow.Add(tiles[x, newY]);
                }
                else
                {
                    break;
                }
            
            }
        }

        if (matchRow.Count >= 3)
        {
            for (int i = 0; i < matchRow.Count; i++)
            {
                finishedMatching.Add(matchRow[i]);

            }
        }

        //列检测
        if (matchRow.Count >= 3) 
        {
            for (int i = 0; i < matchRow.Count; i++)
            {
                //行匹配列表中满足匹配条件的每个元素上下一次进行列遍历检测

                //0代表上方 1带边下方
                for (int j = 0; j <= 1; j++)
                {
                    for (int yDistance = 0; yDistance < Row; yDistance++)
                    {
                        int y;
                        if (j == 0)
                        {
                            y = newY - yDistance;
                        }
                        else
                        {
                            y = newY + yDistance;
                        }
                        if (y < 0 || y >= Row)
                        {
                            break;
                        }
                        if (tiles[matchRow[i].xIndex, y].gridType == curGridType)
                        {
                            matchLine.Add(tiles[matchRow[i].xIndex, y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (matchLine.Count < 2)
                {
                    matchLine.Clear();
                }
                else
                {
                    for (int j = 0; j < matchLine.Count; j++)
                    {
                        finishedMatching.Add(matchLine[j]);


                    }
                    break;
                }
            }

        }
       
        if (finishedMatching.Count >= 3)
        {
            for (int i = 0; i < finishedMatching.Count; i++)
            {
                finishedMatching[i].gameObject.SetActive(false);
            }
            return finishedMatching;
        }

        matchRow.Clear();
        matchLine.Clear();
        finishedMatching.Clear();

        //列匹配
        //i=0代表往左，i=1代表往右
        //for (int i = 0; i <=1; i++)
        //{
        //    for (int yDistance = 0; yDistance < Row; yDistance++)
        //    {
        //        int y;
        //        if (i == 0)
        //        {
        //            y=newY - yDistance;
        //        }
        //        else
        //        {
        //            y = newY + yDistance;
        //        }
        //        if (y < 0 || y >= Row) 
        //        {
        //            break ;
        //        }
        //        if (tiles[newX, y].gridType == curGridType) 
        //        {
        //            matchLine.Add(tiles[newX, y]);
        //        }
        //        else
        //        {
        //            break;
        //        }
                

        //    }
        //}
        //if(matchLine.Count >= 3)
        //{
        //    for (int i = 0; i < matchLine.Count; i++)
        //    {
        //        finishedMatching.Add(matchLine[i]);


        //    }
        //}

        //if (matchLine.Count >= 3)
        //{
        //    for (int i = 0; i < matchLine.Count; i++)
        //    {
        //        for (int j = 0; j <= 1; j++)
        //        {
        //            for (int xDistance = 1; xDistance < Column; xDistance++)
        //            {
        //                int x;
        //                if (j == 0)
        //                {
        //                    x = newY - xDistance;
        //                }
        //                else
        //                {
        //                    x = newY + xDistance;
        //                }
        //                if (x < 0 || x >= Column)
        //                {
        //                    break;
        //                }
        //                if (tiles[x, matchLine[i].yIndex].gridType == curGridType)
        //                {
        //                    matchRow.Add(tiles[x, matchLine[i].yIndex]);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }

        //        }

        //        if (matchRow.Count < 2)
        //        {
        //            matchRow.Clear();
        //        }
        //        else
        //        {
        //            for (int j = 0; j < matchRow.Count; j++)
        //            {
        //                finishedMatching.Add(matchRow[j]);


        //            }
        //            break;
        //        }
        //    }
        //}
        
        if (finishedMatching.Count >= 3)
        {
            for (int i = 0; i < finishedMatching.Count; i++)
            {
                finishedMatching[i].gameObject.SetActive(false);
            }
            return finishedMatching;
        }

        return null;
    }


    /// <summary>
    /// 物品数量用来随机
    /// </summary>
    public int tileCount=5;

    int GetRandomTile()
    {
        int curTileType=Random.Range(0,tileCount);
        return curTileType;
    }

    void AlignGridToBottomCenter()
    {
        // 获取网格整体包围盒
        Bounds bounds = CalculateGridBounds();

        // 计算网格中心点的世界坐标
        Vector3 gridCenter = bounds.center;
        float gridWidth = bounds.size.x;
        float gridHeight = bounds.size.y;

        // 获取摄像机视口底部中心的世界坐标
        Camera mainCamera = Camera.main;
        float bottomY = mainCamera.ViewportToWorldPoint(Vector3.zero).y;
        float centerX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;

        // 计算偏移量以使网格底部居中
        Vector3 offset = new Vector3(centerX - gridCenter.x, bottomY - (gridCenter.y - gridHeight / 2), 0);
        transform.position += offset;
    }

    Bounds CalculateGridBounds()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                bounds.Encapsulate(child.GetComponent<SpriteRenderer>().bounds);
            }
        }
        return bounds;
    }
    /// <summary>
    /// 清除所有背景格
    /// </summary>
    public void ClearAllEmptyGrid()
    {
        int count = allGridRoot.transform.childCount;
        for (int i = 0;i < count; i++)
        {
            //DestroyGrid(GridType.Empty, allGridRoot.GetChild(i).gameObject);
            Destroy(allGridRoot.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 根据位置去计算格子要出现的位置 行  列  间距
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    /// <returns></returns>
    public Vector3 CorrectPositon(int x, int y)
    {
        //Debug.LogError($"当前 x :{x} y : {y} 位置x：{pos.x} 位置y: {pos.y}");
        return new Vector3(transform.position.x - (Row * gridWidth) / 2 + (x * gridWidth), transform.position.y - (Column * gridHeight) / 2 + (y * gridHeight));
    }

    /// <summary>
    /// 供外部获取格子方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetGrid(GridType GridType,GameObject obj)
    {
        //TODO 对象池
        GameObject grid = ObjectPool.Instance.GetObject(GridType);
        if (!grid)
        {
            return grid;
        }
        return CreatGrid(GridType);
    }
    /// <summary>
    /// 加载的格子预制 后面用来clone
    /// </summary>
    private Dictionary<GridType,GameObject> allGridPrefab;

    private void AddPrefab(GridType gridType)
    {
        string path;
        GameObject gridPrefab;
        path = gridType.ToString() + "_Grid";
        gridPrefab = Resources.Load<GameObject>(path);
        if (allGridPrefab.ContainsKey(gridType)) 
        { 
            Debug.LogError($"该类型已经初始化，请不要重复初始化 type:{gridType} path:{path}");
        }
        else
        {
            Debug.Log($"初始化成功 type:{gridType} path:{path}");

            allGridPrefab.Add(gridType, gridPrefab);
        }
    }

    /// <summary>
    /// 创建一个格子
    /// </summary>
    private GameObject CreatGrid(GridType GridType)
    {
        if (allGridPrefab.ContainsKey(GridType))
        {
            GameObject gridPrefab= allGridPrefab[GridType];
            GameObject grid = GameObject.Instantiate(gridPrefab, allGridRoot);
            return grid;
        }
        Debug.LogError("字典中没有加入预制，请初始化");
        return null;
    }

    /// <summary>
    /// 这里的删除 从界面隐藏后 放到对象池供循环使用 //TODO 根据需要看是否放到其他节点下
    /// </summary>
    public void DestroyGrid(GridType GridType,GameObject obj) 
    {
        obj.SetActive(false);
        ObjectPool.Instance.PutObject(GridType, obj);
    }

}
