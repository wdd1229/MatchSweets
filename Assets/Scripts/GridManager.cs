using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TTSDK;
using UnityEngine;
using UnityEngine.Device;

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
    [HideInInspector]
    public Tile[,] BGtiles;

    /// <summary>
    /// 网格和上面的物品的大小差值
    /// </summary>
    private float gridDiff = 30;

    private void Awake()
    {
        allGridRoot = transform;
        allGridPrefab = new Dictionary<GridType, GameObject>();
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
        Debug.Log("--------------------");
        Debug.Log(TT.GetSystemInfo());
        Debug.Log("--------------------");
    }

    private LevelData curLevelData;

    public void GameInit(LevelData levelData)
    {
        curLevelData = levelData;
        Row= curLevelData.Row;
        Column= curLevelData.Column;
        tiles = new Tile[Row, Column + 1];
        BGtiles = new Tile[Row, Column + 1];
        CreatAllEmptyGrid();
        CreateCell();
        visited = new bool[Row, Column + 1];
        removableRegions = new List<List<Tile>>();
        StartCoroutine(StartCheck());
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
        foreach (var items in new List<List<Tile>>(matchTiles))
        {
            foreach (var item in new List<Tile>(items))
            {
                item.SetState(Tile.TileState.Clearing);
                LockTilesAbove(item);
            }
            yield return new WaitForSeconds(0.3f);
        }


        //确定播放结束所有匹配的格子的消除动画
        bool allAnimationsCompleted = false;
        while (!allAnimationsCompleted)
        {
            allAnimationsCompleted = true;
            foreach (List<Tile> items in matchTiles)
            {
                foreach (Tile item in items)
                {
                    if (item != null)
                    {
                        Animator animator = item.GetComponent<Animator>();
                        if (animator != null)
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

        //删除
        foreach (var items in matchTiles)
        {
            if (items == null)
                continue;
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                tiles[item.xIndex, item.yIndex] = null;

                //添加分数
                //AddScore

                Destroy(item.gameObject);
            }
        }

        //等待一段时间确保销毁完成
        yield return new WaitForSeconds(0.3f * matchTiles.Count);

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

            for (int j = 0; j < Column+1; j++)
            {
                Tile currTile = tiles[i, j];
                if (currTile == null)
                {
                    emptySpaceCount++;
                }
                else if (emptySpaceCount > 0)
                {
                    //有空位 移动方块逻辑
                    tiles[i, j - emptySpaceCount] = currTile;
                    tiles[i, j] = null;
                    currTile.Init(i, j - emptySpaceCount, currTile.gridType);
                    currTile.StartFalling(j - emptySpaceCount, emptySpaceCount);
                }
            }


            //yield return new WaitForSeconds(0.2f);
            //根据空格数量生成新的方块
            for (int t = 1; t <= emptySpaceCount; t++)
            {
                //创建新方块
                CreateTileAtTop(i, t, emptySpaceCount);
            }
        }

        // 等待所有方块下落完成
        yield return new WaitForSeconds(0.5f); // 根据下落动画时长调整
    }


    /// <summary>
    /// 生成新格子 设置初始位置和初始化
    /// </summary>
    /// <param name="x"></param>
    /// <param name="startY"></param>
    /// <param name="fallDistance"></param>
    void CreateTileAtTop(int x, int startY, int fallDistance)
    {

        GridType curGridType = (GridType)GetRandomTile();
        GameObject obj = CreatGrid(curGridType);
        Tile tile = obj.AddComponent<Tile>();
        //obj.transform.localPosition=new Vector3(x*tileSize, -startY*tileSize, 0);

        RectTransform grid = obj.GetComponent<RectTransform>();
        grid.sizeDelta = new Vector2(gridItemWidth - gridDiff, gridItemHeight - gridDiff);
        grid.anchoredPosition = new Vector2(
            -totalGridWidth / 2 + gridItemWidth / 2 + x * (gridItemWidth + padding),
           UnityEngine.Screen.height - gridItemHeight / 2 - padding+ startY* (gridItemHeight+padding)
        );
        tile.Init(x, Column + 1 - fallDistance + startY-1, curGridType);
        ////obj.name = string.Format($"Grid_{i}_{j}");
        tiles[x, Column+1- fallDistance+ startY-1] = tile;//Column+1 代表最上面一行 减去空格总数量 加上不同位置startY 因为是数组下标所以再-1
        tile.StartFalling(Column + 1 - fallDistance + startY, fallDistance);//新方块开始掉落
    }

    public void CheckForMatchesAt(int x, int y)
    {
        Tile currentTile = tiles[x, y];
        if (currentTile == null) return;
        //Debug.LogError("落下之后的检测匹配");
        List<List<Tile>> matchedTiles = CheckForRemovableRegion();
        //Debug.LogError("------CheckForMatchesAt-----1");

        if (matchedTiles.Count == 0)
        {
            //Debug.LogError("----没有匹配格子了----");
            //没有匹配的格子之后再去生成 

            return;
        }
        //Debug.LogError("------CheckForMatchesAt-----2");

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
                if (!visited[i, j] && tiles[i, j] != null)
                {
                    regions = new List<Tile>();
                    int count = DFS(i, j, tiles[i, j].gridType, regions);

                    if (count >= 3 && IsLinear(regions))
                    {
                        //Debug.LogError("------------有可消除");
                        removableRegions.Add(regions);
                    }
                }
            }
        }
        return removableRegions;
    }

    public float padding = 10f; // 网格项之间的间距
    public float horizontalMargin = 50f; // 预留的左右边距

    [HideInInspector]
    public float gridItemWidth;
    [HideInInspector]
    public float gridItemHeight;
    [HideInInspector]
    public float totalGridWidth;
    [HideInInspector]
    public float totalGridHeight;
    /// <summary>
    /// 创建所有空格子 也就是背景格
    /// </summary>
    public void CreatAllEmptyGrid()
    {
        Array.Clear(BGtiles, 0, BGtiles.Length);

        RectTransform canvasRect = transform.parent as RectTransform;
        float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        gridItemWidth = (availableWidth - padding * (Column - 1)) / Column;
        gridItemHeight = gridItemWidth; // 保持正方形比例

        // 设置网格容器的大小
        RectTransform gridContainer = GetComponent<RectTransform>();
        totalGridWidth = Column * gridItemWidth + (Column - 1) * padding;
        //totalGridHeight = Row * gridItemHeight + (Row - 1) * padding;

        Debug.Log("当前屏幕高度为：" + UnityEngine.Screen.height);
        totalGridHeight = UnityEngine.Screen.height;
        gridContainer.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        // 设置网格容器的位置，使其在屏幕下方居中
        gridContainer.anchorMin = new Vector2(0.5f, 0f);
        gridContainer.anchorMax = new Vector2(0.5f, 0f);
        gridContainer.pivot = new Vector2(0.5f, 0f);
        gridContainer.anchoredPosition = new Vector2(0f, 0f); // 底部居中


        GameObject obj;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column+1; j++)
            {
                obj = CreatGrid(GridType.Empty);

                RectTransform grid = obj.GetComponent<RectTransform>();
                grid.sizeDelta = new Vector2(gridItemWidth, gridItemHeight);

                Tile tile = obj.AddComponent<Tile>();

                if (j == Column)
                {
                    tile.Init(i, j, GridType.Top);
                    BGtiles[i, j] = tile;

                    grid.localPosition = new Vector3(-totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding), UnityEngine.Screen.height- gridItemHeight/2- padding, 0);

                }
                else
                {

                    tile.Init(i, j, GridType.Empty);
                    BGtiles[i, j] = tile;

                    grid.anchoredPosition = new Vector2(
                   -totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding),
                   -totalGridHeight / 2 + gridItemHeight / 2 + j * (gridItemHeight + padding)
                    );
                }


            }
        }
    }


    float fullScreenSpawnChance = 0.5f; // 50% 的概率 全屏同一类型几率
    //float fullScreenSpawnChance = 0.05f; // 5% 的概率 全屏同一类型几率

    /// <summary>
    /// 创建游戏物品 在格子背景上的
    /// </summary>
    public void CreateCell()
    {
        GameObject obj;
        GridType curGridType=GridType.Null;

        bool isFullScreen = false;//是否全屏同一类型

        if (UnityEngine.Random.value < fullScreenSpawnChance)
        {
            // 触发全屏同类型生成逻辑
            curGridType = (GridType)GetRandomTile();
            isFullScreen =true;
        }
        else
        {
            // 正常随机生成逻辑
            isFullScreen = false;
        }

        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column+1; j++)
            {
                if (!isFullScreen)
                {
                    curGridType= (GridType)GetRandomTile();
                }
                obj = CreatGrid(curGridType);
                Tile tile = obj.AddComponent<Tile>();
                tile.Init(i, j, curGridType);
                //obj.transform.localPosition = new Vector3(i * tileSize, -j * tileSize, 0);
                //obj.name = string.Format($"Grid_{i}_{j}");
                tiles[i, j] = tile;

                RectTransform grid = obj.GetComponent<RectTransform>();

                grid.sizeDelta = new Vector2(gridItemWidth - gridDiff, gridItemHeight - gridDiff);

                grid.transform.localPosition=BGtiles[i, j].transform.localPosition;
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
    private int DFS(int row, int col, GridType tileType, List<Tile> regon)
    {
        // 检查边界和Tile类型是否匹配
        if (row < 0 || row >= Row || col < 0 || col >= Column || tiles[row, col] == null || visited[row, col] || tiles[row, col].gridType != tileType)
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
    public int tileCount = 5;

    /// <summary>
    /// 随机不同类型格子
    /// </summary>
    /// <returns></returns>
    int GetRandomTile()
    {
        int curTileType = UnityEngine.Random.Range(0, tileCount);
        return curTileType;
    }

    
    /// <summary>
    /// 清除所有背景格
    /// </summary>
    public void ClearAllEmptyGrid()
    {
        int count = allGridRoot.transform.childCount;
        for (int i = 0; i < count; i++)
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
    public GameObject GetGrid(GridType GridType, GameObject obj)
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
    private Dictionary<GridType, GameObject> allGridPrefab;

    private void AddPrefab(GridType gridType)
    {
        string path;
        GameObject gridPrefab;
        path = "Test/" + gridType.ToString() + "_Grid";
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
            GameObject gridPrefab = allGridPrefab[GridType];
            GameObject grid = GameObject.Instantiate(gridPrefab, allGridRoot);
            return grid;
        }
        Debug.LogError("字典中没有加入预制，请初始化");
        return null;
    }

    /// <summary>
    /// 这里的删除 从界面隐藏后 放到对象池供循环使用 //TODO 根据需要看是否放到其他节点下
    /// </summary>
    public void DestroyGrid(GridType GridType, GameObject obj)
    {
        obj.SetActive(false);
        ObjectPool.Instance.PutObject(GridType, obj);
    }

}
