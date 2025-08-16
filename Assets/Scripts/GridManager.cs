using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float gridWidth;
    /// <summary>
    /// 格子高度
    /// </summary>
    private float gridHeight;

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


    public Vector2 referenceResolution = new Vector2(1920,1080); // 参考分辨率


    private void Start()
    {
        //float currentWidth = Screen.width;
        //float currentHeight = Screen.height;
        //float scaleX = currentWidth / referenceResolution.x;
        //float scaleY = currentHeight / referenceResolution.y;
        //transform.localScale = new Vector3(scaleX, scaleY, 1f);


        tiles =new Tile[Row, Column];

        CreatAllEmptyGrid();

        AlignGridToBottomCenter();

        CreateCell();


        visited = new bool[Row, Column];
        removableRegions = new List<List<Tile>>();

        StartCoroutine(StartCheck());

        //TODO
        //removableRegions = CheckForRemovableRegion();
        //foreach (var region in removableRegions)
        //{
        //    foreach (var cell in region)
        //    {
        //        cell.gameObject.SetActive(false);
        //    }
        //}
    }

    [ContextMenu("CreateCell")]
    public void Test()
    {
        Array.Clear(visited, 0, visited.Length);
        removableRegions.Clear();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                if (tiles[i, j] != null)
                {
                    Destroy(tiles[i, j].gameObject);
                    tiles[i, j] = null;
                }
            }
        }
        CreateCell();

        removableRegions = CheckForRemovableRegion();
       
    }
    [ContextMenu("AllMatchDisable")]
    public void AllMatchDisable()
    {
        foreach (var region in removableRegions)
        {
            foreach (var cell in region)
            {
                cell.gameObject.SetActive(!cell.gameObject.activeSelf);
            }
        }
    }



    IEnumerator StartCheck()
    {

        yield return new WaitForSeconds(1.5f);

        removableRegions = CheckForRemovableRegion();

        StartCoroutine(ClearAllMatchGrid(removableRegions));
    }

    /// <summary>
    /// 清除所有已经匹配格子
    /// </summary>
    IEnumerator ClearAllMatchGrid(List<List<Tile>> matchTiles)
    {
        Debug.LogError("-----ClearAllMatchGrid----1");
        foreach (var items in new List<List<Tile>>(matchTiles))
        {
            foreach (var item in new List<Tile>(items))
            {
                item.SetState(Tile.TileState.Clearing);
                LockTilesAbove(item);
            }
            yield return new WaitForSeconds(0.3f);
        }

        Debug.LogError("-----ClearAllMatchGrid----2");

        //确定播放结束所有匹配的格子的消除动画
        bool allAnimationsCompleted = false;
        while (!allAnimationsCompleted)
        {
            allAnimationsCompleted = true;
            foreach (List<Tile> items in matchTiles)
            {
                foreach(Tile item in items)
                {
                    if (item != null)
                    {
                        Animator animator = item.GetComponent<Animator>();
                        if(animator != null)
                        {
                            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                            if (!stateInfo.IsName("clear") || stateInfo.normalizedTime < 1.0f)
                            {
                                allAnimationsCompleted = false;
                                break;
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        Debug.LogError("-----ClearAllMatchGrid----2");

        //删除
        foreach (var items in matchTiles)
        {
            foreach (var item in items)
            {
                tiles[item.xIndex,item.yIndex]=null;

                //添加分数
                //AddScore

                Destroy(item.gameObject);
            }
        }

        //等待一段时间确保销毁完成
        yield return new WaitForSeconds(0.3f* matchTiles.Count);

        RefillBoard();
    }

    void LockTilesAbove(Tile tile)
    {
        int tileYIndex = tile.yIndex;
        for (int y = 0; y < tileYIndex; y++)
        {
            Tile tileAbove = tiles[tile.xIndex, y];
            if (tileAbove != null)
            {
                tileAbove.SetState(Tile.TileState.Moving); // 锁定上方的方块
            }
        }
    }

    void RefillBoard()
    {
        StartCoroutine(RefillBoardRoutine());
    }

    IEnumerator RefillBoardRoutine()
    {
        int emptySpaceCount;
        for (int i = 0; i < Row; i++)
        {
            //空格子数量统计
            emptySpaceCount = 0;

            for(int j = Column-1; j >= 0; j--)
            {
                Tile currTile = tiles[i,j];
                if (currTile == null) 
                {
                    emptySpaceCount++;
                }else if (emptySpaceCount > 0)
                {
                    //有空位 移动方块逻辑
                    tiles[i,j+emptySpaceCount]=currTile;
                    tiles[i, j] = null;
                    currTile.Init(i, j + emptySpaceCount, currTile.gridType);
                    currTile.StartFalling(emptySpaceCount);
                }
            }
            Debug.LogError($"要创建新的方块数量为:{emptySpaceCount}");
            //yield return new WaitForSeconds(0.2f);
            //根据空格数量生成新的方块
            for (int t = 1; t <= emptySpaceCount; t++)
            {
                //创建新方块
                CreateTileAtTop(i, -t, emptySpaceCount);
            }

        }

        // 等待所有方块下落完成
        yield return new WaitForSeconds(0.5f); // 根据下落动画时长调整
    }

    public float creatTileHeight = 0;

    /// <summary>
    /// 生成新格子 设置初始位置和初始化
    /// </summary>
    /// <param name="x"></param>
    /// <param name="startY"></param>
    /// <param name="fallDistance"></param>
    void CreateTileAtTop(int x,int startY,int fallDistance)
    {       

        GridType curGridType = (GridType)GetRandomTile();
        GameObject obj = CreatGrid(curGridType);
        Tile tile = obj.AddComponent<Tile>();
        //obj.transform.localPosition=new Vector3(x*tileSize, -startY*tileSize, 0);
        obj.transform.localPosition=new Vector3(x*tileSize, creatTileHeight, 0);


        tile.Init(x, startY+fallDistance, curGridType);
        ////obj.name = string.Format($"Grid_{i}_{j}");
        tiles[x, startY+fallDistance] = tile;
        tile.StartFalling(fallDistance);//新方块开始掉落
    }

    public void CheckForMatchesAt(int x,int y)
    {
        Tile currentTile = tiles[x, y];
        if(currentTile == null)return;
        Debug.LogError("落下之后的检测匹配");
        List<List<Tile>> matchedTiles = CheckForRemovableRegion();
        Debug.LogError("------CheckForMatchesAt-----1");

        if (matchedTiles.Count == 0) 
        {
            Debug.LogError("----没有匹配格子了----");

            //没有匹配的格子之后再去生成 


            return;
        }
        Debug.LogError("------CheckForMatchesAt-----2");

        StartCoroutine(ClearAllMatchGrid(matchedTiles));
    }

    


    private List<List<Tile>> removableRegions;

    public static float tileSize = 1.024f;

    /// <summary>
    /// 检查所有格子判断是否有可消除的
    /// </summary>
    /// <returns></returns>
    public List<List<Tile>> CheckForRemovableRegion()
    {
        Array.Clear(visited, 0, visited.Length);
        removableRegions.Clear();
        List<Tile> regions;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                if (!visited[i, j] && tiles[i, j]!=null)
                {
                    regions = new List<Tile>();
                    int count = DFS(i, j, tiles[i, j].gridType, regions);

                    if (count>=3 && IsLinear(regions))
                    {
                        Debug.LogError("------------有可消除");                 
                        removableRegions.Add(regions);
                    }
                }
            }
        }
        return removableRegions;
    }

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
    Dictionary<int, int> rowCounts = new Dictionary<int, int>();
    Dictionary<int, int> colCounts = new Dictionary<int, int>();
    /// <summary>
    /// 用来排除双排双列  一加二
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    private bool IsLinear(List<Tile> tiles)
    {
        rowCounts.Clear();
        colCounts.Clear();
        foreach (var tile in tiles)
        {
            if (!rowCounts.ContainsKey(tile.xIndex))
            {
                rowCounts[tile.xIndex] = 0;
            }
            rowCounts[tile.xIndex]++;

            if (!colCounts.ContainsKey(tile.yIndex))
            {
                colCounts[tile.yIndex] = 0;
            }
            colCounts[tile.yIndex]++;
        }

        foreach (var rowValue in rowCounts.Values)
        {
            if (rowValue >= 3)
            {
                return true;
            }
        }

        foreach (var colValue in colCounts.Values)
        {
            if (colValue >= 3)
            {
                return true;
            }
        }
        return false;
    }


    private bool[,] visited; // 访问标记数组

    // 深度优先搜索（DFS）用于统计相连区域的大小
    private int DFS(int row, int col, GridType tileType,List<Tile> regon)
    {
        // 检查边界和Tile类型是否匹配
        if (row < 0 || row >= Row || col < 0 || col >= Column ||  tiles[row, col] == null || visited[row, col] || tiles[row, col].gridType != tileType)
        {
            return 0;
        }

        visited[row, col] = true;

        regon.Add(tiles[row, col]);

        // 向四个方向递归遍历
        int count = 1;
        count += DFS(row - 1, col, tileType, regon); // 上
        count += DFS(row + 1, col, tileType, regon); // 下
        count += DFS(row, col - 1, tileType, regon); // 左
        count += DFS(row, col + 1, tileType, regon); // 右

        return count;
    }


    /// <summary>
    /// 物品数量用来随机
    /// </summary>
    public int tileCount=5;

    /// <summary>
    /// 随机不同类型格子
    /// </summary>
    /// <returns></returns>
    int GetRandomTile()
    {
        int curTileType=UnityEngine.Random.Range(0,tileCount);
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

        Debug.LogError($"--AlignGridToBottomCenter-- gridWidth:{gridWidth} gridHeight:{gridHeight} {bounds.center}");

        // 获取摄像机视口底部中心的世界坐标
        Camera mainCamera = Camera.main;
        float bottomY = mainCamera.ViewportToWorldPoint(Vector3.zero).y;
        float centerX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;

        // 计算偏移量以使网格底部居中
        Vector3 offset = new Vector3(centerX - gridCenter.x, bottomY - (gridCenter.y - gridHeight / 2), 0);
        transform.position += offset;

        //transform.position += new Vector3(0, 100, 0);

        //判断是否超过手机界面的宽度
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
