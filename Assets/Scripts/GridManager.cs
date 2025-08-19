using System;
using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
using UnityEngine.Windows;
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
    ///全部的方块
    /// </summary>
    private Tile[,] tiles;
    /// <summary>
    ///背景格子
    /// </summary>
    [HideInInspector]
    public Tile[,] BGtiles;

    /// <summary>
    ///网格和上面的物品的大小差值
    /// </summary>
    private float gridDiff = 30;


    public float padding = 10f; //  网格项之间的间距
    public float horizontalMargin = 50f; // 预留的左右边距

    [HideInInspector]
    public float gridItemWidth;
    [HideInInspector]
    public float gridItemHeight;
    [HideInInspector]
    public float totalGridWidth;
    [HideInInspector]
    public float totalGridHeight;


    float fullScreenSpawnChance = 0.5f; // 50% 的概率 全屏同一类型几率

    /// <summary>
    /// 网格和上面的物品的大小差值
    /// </summary>
    public int tileCount = 5;

    float SpecialCollection = 0.20f; // 5*4% 的概率 特殊收集品概率


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
        AddPrefab(GridType.SpecialCollection);
    }


    private void Start()
    {
        Debug.Log("--------------------");
        Debug.Log(TT.GetSystemInfo());
        Debug.Log("--------------------");
    }


    public void GameInit(LevelData levelData)
    {

        Row = levelData.Row;
        Column= levelData.Column;
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


        //StartCoroutine(ClearAllSpecialCollection(CheckForSpecialCollection()));

        StartCoroutine(ClearAllMatchGrid(removableRegions));
    }

    IEnumerator ClearAllSpecialCollection(List<Tile> specialTiles)
    {
        if (specialTiles.Count == 0)
            yield return null;

        Debug.LogError($"-------�����ռ�Ʒ������{specialTiles.Count}-------");

        foreach (var item in new List<Tile>(specialTiles))
        {
            item.SetState(Tile.TileState.Clearing);
            LockTilesAbove(item);
        }
        //ȷ�����Ž�������ƥ��ĸ��ӵ���������
        bool allAnimationsCompleted = false;
        while (!allAnimationsCompleted)
        {
            allAnimationsCompleted = true;
            foreach (Tile item in specialTiles)
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
            yield return null;
        }

        //ɾ��
        foreach (var item in specialTiles)
        {      
                if (item == null)
                    continue;
                tiles[item.xIndex, item.yIndex] = null;

            //���ӷ���
            //AddSpecial
            Debug.LogError("-------ɾ�������ռ�Ʒ-------");
            Destroy(item.gameObject);
        }

        //�ȴ�һ��ʱ��ȷ���������
        yield return new WaitForSeconds(0.3f);

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

                if (item.gridType == GridType.SpecialCollection)
                {
                    //AddSpecial
                    GameManager.Instance.RefreshSpecial();
                }
                else
                {
                    //添加分数
                    //AddScore
                }
                tiles[item.xIndex, item.yIndex] = null;

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

       

        //等待所有方块下落完成
        yield return new WaitForSeconds(1.5f); //根据下落动画时长调整



        CheckForMatchesAt();
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

    public List<Tile> curMatchedTiles;
    public void CheckForMatchesAt(/*int x, int y*/)
    {
        //Tile currentTile = tiles[x, y];
        //if (currentTile == null) return;
        List<List<Tile>> matchedTiles = CheckForRemovableRegion();
        //Debug.LogError("------CheckForMatchesAt-----1");

        if (matchedTiles.Count == 0)
        {
            //Debug.LogError("----没有匹配格子了----");
            //没有匹配的格子之后再去生成 

            return;
        }
        
        //StartCoroutine(ClearAllSpecialCollection(CheckForSpecialCollection()));


        StartCoroutine(ClearAllMatchGrid(matchedTiles));
    }




    private List<List<Tile>> removableRegions;

    /// <summary>
    /// 检查所有格子判断是否有可消除的
    /// </summary>
    /// <returns></returns>
    public List<List<Tile>> CheckForRemovableRegion()
    {
        Array.Clear(visited, 0, visited.Length);
        removableRegions.Clear();
        List<Tile> regions;

        List<Tile> specialTiles=new List<Tile>();
        specialTiles.Clear();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                if(tiles[i, j] != null && tiles[i, j].gridType == GridType.SpecialCollection)
                {
                    specialTiles.Add(tiles[i, j]);
                }

                if (!visited[i, j] && tiles[i, j] != null)
                {
                    regions = new List<Tile>();
                    int count = DFS(i, j, tiles[i, j].gridType,ref regions);

                    if (count >= 3 && IsLinear(regions))
                    {
                        //Debug.LogError("------------有可消除");
                        removableRegions.Add(regions);
                    }
                }
            }
        }
        if(specialTiles.Count>0)
            removableRegions.Add(specialTiles);
        return removableRegions;
    }

    /// <summary>
    /// ����Ƿ��������ղ�Ʒ
    /// </summary>
    /// <returns></returns>
    private  List<Tile> CheckForSpecialCollection()
    {
        List<Tile> SpecialTiles=new List<Tile>();
        for (int i = 0; i<Row; i++)
        {
            for (int j = 0; j<Column; j++)
            {
                if (tiles[i, j] != null && tiles[i,j].gridType==GridType.SpecialCollection)
                {
                    SpecialTiles.Add(tiles[i, j]);
                }
            }
        }
        return SpecialTiles;
    }



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
        Debug.Log("当前Canvas高度为：" + GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.height);

        //RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        //float topPosition = canvasRect.rect.height * 0.5f; // ��ȡCanvas�߶ȵ�һ�룬������λ��
        Rect safeArea = UnityEngine.Screen.safeArea;
        float topInset = UnityEngine.Screen.height - safeArea.yMax;

        totalGridHeight = UnityEngine.Screen.height;
        gridContainer.sizeDelta = new Vector2(totalGridWidth, UnityEngine.Screen.height);

        // ��������������λ�ã�ʹ������Ļ�·�����
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
            curGridType = (GridType)GetRandomTile(true);
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
    private int DFS(int row, int col, GridType tileType,ref List<Tile> regon)
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
        count += DFS(row - 1, col, tileType,ref regon); // ��
        count += DFS(row + 1, col, tileType,ref regon); // ��
        count += DFS(row, col - 1, tileType,ref regon); // ��
        count += DFS(row, col + 1, tileType,ref regon); // ��

        return count;
    }


    /// <summary>
    /// �����ͬ���͸���
    /// </summary>
    /// 随机不同类型格子
    /// <returns></returns>
    int GetRandomTile(bool isFrist=false)
    {
        //isFrist �����һ��ʱ�ͳ��� �����ռ�Ʒȫ�������
        if (UnityEngine.Random.value < SpecialCollection && isFrist==false)
        {
            return (int)GridType.SpecialCollection;
            //return UnityEngine.Random.Range(0, tileCount);
        }
        else
        {
            return UnityEngine.Random.Range(0, tileCount);
        }

          
        
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
    public Dictionary<GridType, GameObject> allGridPrefab;

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
