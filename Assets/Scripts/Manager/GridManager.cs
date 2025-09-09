using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TTSDK;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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
    private int Column;
    /// <summary>
    /// 行数
    /// </summary>
    private int Row;

    /// <summary>
    ///全部的方块
    /// </summary>
    private Tile[,] tiles;
    /// <summary>
    ///背景格子
    /// </summary>
    //[HideInInspector]
    //public Tile[,] BGtiles;

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

    public Transform canvas;

    private Transform floatingRoot;
    private void Awake()
    {
        floatingRoot = canvas.Find("GameUI/floatingRoot");
        allGridRoot = transform;
    }


    private void Start()
    {
        Debug.Log("--------------------");
        Debug.Log(TT.GetSystemInfo());
        Debug.Log("--------------------");
    }


    public void GameInit(LevelData levelData)
    {
        Debug.LogError($"GridManager---GameInit--{JsonUtility.ToJson(levelData,true)}");

        Row = levelData.Row;
        Column= levelData.Column;
        tiles = new Tile[Row, Column + 1];

        visited = new bool[Row, Column + 1];
        removableRegions = new List<List<Tile>>();

        allRowEmptyDic = new List<Tile>();

        StartCoroutine(CreatGrid());

    }

    /// <summary>
    /// 使用IEnumerator方式让两个协程顺序执行 避免数据混乱
    /// </summary>
    /// <param name="levelData"></param>
    /// <returns></returns>
    public IEnumerator GameReset(LevelData levelData)
    {
        yield return StartCoroutine(ClearAllEmptyGrid(levelData));

        yield return StartCoroutine(CreatGrid());
    }

    public void StorageClear(LevelData levelData)
    {
        Array.Clear(tiles, 0, tiles.Length);
        Array.Clear(visited, 0, tiles.Length);
        removableRegions.Clear();

        Row = levelData.Row;
        Column = levelData.Column;
        tiles = new Tile[Row, Column + 1];
        visited = new bool[Row, Column + 1];

    }

    IEnumerator StartCheck()
    {
        yield return new WaitForSeconds(1.5f);

        removableRegions = CheckForRemovableRegion();

        //if (removableRegions.Count > 0)
            StartCoroutine(ClearAllMatchGrid(removableRegions));
        //StartCoroutine(ClearAllSpecialCollection(CheckForSpecialCollection()));

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

        int specialScore = 0;
        int normalScore = 0;
        //删除
        foreach (var items in matchTiles)
        {
            if (items == null || items.Count==0)
                continue;

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                if (item.gridType == GridType.SpecialCollection)
                {
                    specialScore++;
                    //AddSpecial
                    GameManager.Instance.RefreshSpecial();

                    //yield return new WaitForSeconds(2f);

                    if (GameManager.Instance.IsNextLevelCheck())
                    {
                        Debug.LogError($"下一关来来来 {GameLevelManager.Instance.GetCurLevelIndex()},{GameLevelManager.Instance.GetLevelData().levels.Length}");

                        if(GameLevelManager.Instance.GetCurLevelIndex()>= GameLevelManager.Instance.GetLevelData().levels.Length - 1)
                        {
                            //进入龙珠夺宝阶段
                            GameManager.Instance.ShowReward();
                        }
                        else
                        {
                            //下一关弹窗
                            GameManager.Instance.ShowLevelPopup();  
                        }
                        //GameManager.Instance.GameReset();
                        yield break;
                    }
                }
                tiles[item.xIndex, item.yIndex] = null;

                Destroy(item.gameObject);
            }

            //添加分数
            //AddScore
            normalScore += GameManager.Instance.RefreshScore(items[0].gridType, items.Count);
        }
        if (specialScore > 0)
            (PrefabManager.Instance.InstantiatefloatingPrefab(FloatingType.specialFloating, specialScore, floatingRoot, Vector3.zero)).transform.localPosition = new Vector3(0, 150, 0);

        if (normalScore > 0)
            (PrefabManager.Instance.InstantiatefloatingPrefab(FloatingType.normalFloating, normalScore * 10, floatingRoot, Vector3.zero)).transform.localPosition = new Vector3(0, 0, 0);

        //等待一段时间确保销毁完成
        yield return new WaitForSeconds(0.5f * matchTiles.Count);

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

    List<Tile> allRowEmptyDic;
    IEnumerator RefillBoardRoutine()
    {
        //Dictionary<int,int> allRowEmptyDic=new Dictionary<int, int>();
        allRowEmptyDic.Clear();

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

                    allRowEmptyDic.Add(currTile);


                }
            }


            //yield return new WaitForSeconds(0.9f);
            //根据空格数量生成新的方块
            for (int t = 1; t <= emptySpaceCount; t++)
            {
                yield return new WaitForSeconds(0.1f);
                //创建新方块
                CreateTileAtTop(i, t, emptySpaceCount);
            }
        }
        ////等待所有方块下落完成
        yield return new WaitForSeconds(1.5f); //根据下落动画时长调整
        CheckForMatchesAt();
    }

    public IEnumerator CreatGrid()
    {
        int x=UnityEngine.Random.Range(0, Column);
        int y=UnityEngine.Random.Range(0, Row);

        for (int i = 0; i < Column + 1; i++)
        {
            for (int t = 0; t < Row; t++)
            {
                if (x == i && y == t) 
                {
                    CreateTileAtTop(t, i, Column,true);
                }
                else
                {
                    //创建新方块
                    CreateTileAtTop(t, i, Column);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        StartCoroutine(StartCheck());
    }


    /// <summary>
    /// 生成新格子 设置初始位置和初始化
    /// </summary>
    /// <param name="x"></param>
    /// <param name="startY"></param>
    /// <param name="fallDistance"></param>
    void CreateTileAtTop(int x, int startY, int fallDistance,bool IsSpecial=false)
    {

        GridType
        curGridType = (GridType)GetRandomTile(IsSpecial);

        //GameObject obj = CreatGrid(curGridType);
        GameObject obj=PrefabManager.Instance.InstantiateGridPrefab(curGridType.ToString() + "_Grid", allGridRoot);

        Tile tile = obj.AddComponent<Tile>();

        RectTransform grid = obj.GetComponent<RectTransform>();
        //grid.anchorMin = new Vector2(0, 0);
        //grid.anchorMax = new Vector2(0, 0);
        //grid.pivot = new Vector2(0, 0);

        //grid.sizeDelta = new Vector2(gridItemWidth - gridDiff, gridItemHeight - gridDiff);
        //grid.anchoredPosition = new Vector2(
        //    -totalGridWidth / 2 + gridItemWidth / 2 + x * (gridItemWidth + padding),
        //   UnityEngine.Screen.height - gridItemHeight / 2 - padding+ startY* (gridItemHeight+padding)
        //);
        grid.sizeDelta = new Vector2(120,120);
        //grid.localPosition = BGtiles[x, Column + 1 - fallDistance + startY - 1].transform.localPosition ;
        //每个格子上生成的效果
        //grid.anchoredPosition = CalculateGridPos(x, Column + 1 - fallDistance + startY - 1);
        //从上面掉落的效果
        grid.anchoredPosition = CalculateGridPos(x, Column);

        //Debug.LogError($"初始位置 x:{x} y:{Column + 1 - fallDistance + startY - 1}");

        tile.Init(x, Column + 1 - fallDistance + startY-1, curGridType);
        ////obj.name = string.Format($"Grid_{i}_{j}");
        tiles[x, Column+1- fallDistance+ startY-1] = tile;//Column+1 代表最上面一行 减去空格总数量 加上不同位置startY 因为是数组下标所以再-1 
        tile.StartFalling(Column + 1 - fallDistance + startY, fallDistance);//新方块开始掉落

       
    }

    private List<Tile> curMatchedTiles;
    public void CheckForMatchesAt(/*int x, int y*/)
    {
        //Tile currentTile = tiles[x, y];
        //if (currentTile == null) return;
        List<List<Tile>> matchedTiles = CheckForRemovableRegion();
        //Debug.LogError("------CheckForMatchesAt-----1");

        if (matchedTiles.Count == 0)
        {
            Debug.LogError("----没有匹配格子了----");

            if (GameManager.Instance.GetAiState())
            {
                StartCoroutine(AiStartCheck());
                GameManager.Instance.gameUi.SetAIBtnState(false);
                GameManager.Instance.gameUi.SetResetBtnState(true);
            }
            else
            {
                GameManager.Instance.gameUi.SetAIBtnState(true);
                GameManager.Instance.gameUi.SetResetBtnState(true);
            }

            //没有匹配的格子之后再去生成 
            return;
        }
        //StartCoroutine(ClearAllSpecialCollection(CheckForSpecialCollection()));
        StartCoroutine(ClearAllMatchGrid(matchedTiles));
    }

    IEnumerator AiStartCheck()
    {
        yield return new WaitForSeconds(3);
        if (GameManager.Instance.GetAiState())
        {
            GameManager.Instance.gameUi.SetResetBtnState(false);
            GameManager.Instance.gridManager.TriggerExplosion();
            //StartCoroutine(GameManager.Instance.gridManager.GameReset(GameLevelManager.Instance.GetCurLevel()));
            GameManager.Instance.ResetGrid();
        }
        else
        {
            GameManager.Instance.gameUi.SetResetBtnState(false);
            GameManager.Instance.gameUi.SetResetBtnState(false);
        }
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

    private int width;
    private int height;
    private Vector3 origin;           // 网格原点（左下角）
    private float cellSize = 120;      // 网格单元大小
    public Vector2 CalculateGridPos(int xIndex,int yIndex)
    {
        Vector2 tt = canvas.GetComponent<RectTransform>().sizeDelta;
        origin = allGridRoot.position;
        width = (int)(tt.x / cellSize);
        height = (int)(tt.y / cellSize);
        //width = (int)(Screen.width / cellSize);
        //height = (int)(Screen.height / cellSize);

        //Debug.LogError($"当前屏幕生成网格 width:{width} height:{height}");

        float startX = (width - Row) / 2.0f;
        if (yIndex == Column)
        {
            yIndex = height-3;
        }
        return GridToWorld(new Vector2(xIndex+ startX, yIndex));

        if (yIndex == Column)
        {
            return new Vector2(-totalGridWidth / 2 + gridItemWidth / 2 + xIndex * (gridItemWidth + padding), totalGridHeight/2 - gridItemHeight / 2 - padding);
        }
        else
        {

        }

        return new Vector2(
                   -totalGridWidth / 2 + gridItemWidth / 2 + xIndex * (gridItemWidth + padding),
                   -totalGridHeight / 2 + gridItemHeight / 2 + yIndex * (gridItemHeight + padding)/*+Screen.height/2*/
                    );
    }

    // 将世界坐标转换为网格坐标
    public Vector2 WorldToGrid(Vector3 worldPos)
    {
        float x = ((worldPos - origin).x / cellSize);
        float y = ((worldPos - origin).y / cellSize);
        return new Vector2(x, y);
    }

    // 将网格坐标转换为世界坐标
    public Vector3 GridToWorld(Vector2 gridPos)
    {
        return new Vector3(
            gridPos.x * cellSize + origin.x + cellSize / 2,
            gridPos.y * cellSize + origin.y + cellSize / 2,
            origin.z
        );
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
    int GetRandomTile(bool isFullScreen = false)
    {


        //if (UnityEngine.Random.value < SpecialCollection && isFrist==false)
        if(isFullScreen)
        {
            return (int)GridType.SpecialCollection;
        }
        else
        {
            return UnityEngine.Random.Range(0, tileCount);
        }
    }

    
    /// <summary>
    /// 清除所有格子 包括背景和所有类型格子
    /// </summary>
    public IEnumerator ClearAllEmptyGrid(LevelData levelData)
    {
        Debug.LogError("ClearAllEmptyGrid");
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column+1; j++)
            {
                if(tiles[i, j]!=null)
                    tiles[i, j].SetState(Tile.TileState.Clearing);
            }
        }
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column + 1; j++)
            {
                if (tiles[i, j] != null)
                {

                    Destroy(tiles[i, j].gameObject);
                    tiles[i,j]=null;
                }
            }
        }
        yield return new WaitForSeconds(1.5f);

        //foreach (var items in new List<List<Tile>>(matchTiles))
        //{
        //    foreach (var item in new List<Tile>(items))
        //    {
        //        item.SetState(Tile.TileState.Clearing);
        //        LockTilesAbove(item);
        //    }
        //}
        if(levelData!=null)
            StorageClear(levelData);

        ////确定播放结束所有匹配的格子的消除动画
        //bool allAnimationsCompleted = false;
        //while (!allAnimationsCompleted)
        //{
        //    allAnimationsCompleted = true;
        //    foreach (List<Tile> items in matchTiles)
        //    {
        //        foreach (Tile item in items)
        //        {
        //            if (item != null)
        //            {
        //                Animator animator = item.GetComponent<Animator>();
        //                if (animator != null)
        //                {
        //                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //                    if (!stateInfo.IsName("clear") || stateInfo.normalizedTime < 1.0f)
        //                    {
        //                        allAnimationsCompleted = false;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    yield return null;
        //}

        //int count = allGridRoot.transform.childCount;
        //for (int i = count-1; i >= 0; i--)
        //{
        //    //DestroyGrid(GridType.Empty, allGridRoot.GetChild(i).gameObject);

        //    Destroy(allGridRoot.GetChild(i).gameObject);
        //}
    }

    /// <summary>
    /// 爆炸效果
    /// </summary>
    public void TriggerExplosion()
    {
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column + 1; j++)
            {
                if (tiles[i, j] != null)
                {

                    // 随机方向（偏向上方）
                    Vector2 dir = new Vector2(
                        UnityEngine.Random.Range(-0.8f, 0.8f),
                        UnityEngine.Random.Range(0.5f, 1f)
                    );
                    tiles[i,j].StartExplosion(dir);
                }
            }
        }
        //foreach (TileTest tile in tiles)
        //{
            
        //}
    }
}
