using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TTSDK;
using UnityEngine;
using UnityEngine.Device;

/// <summary>
/// ���и��ӹ���
/// </summary>
public class GridManager : MonoBehaviour
{

    /// <summary>
    /// ���и��Ӹ��ڵ�
    /// </summary>
    private Transform allGridRoot;

    /// <summary>
    /// ����
    /// </summary>
    public int Column;
    /// <summary>
    /// ����
    /// </summary>
    public int Row;

    /// <summary>
    /// ���ӿ��
    /// </summary>
    private float gridWidth;
    /// <summary>
    /// ���Ӹ߶�
    /// </summary>
    private float gridHeight;

    /// <summary>
    /// ȫ���ķ���
    /// </summary>
    private Tile[,] tiles;
    [HideInInspector]
    public Tile[,] BGtiles;

    /// <summary>
    /// ������������Ʒ�Ĵ�С��ֵ
    /// </summary>
    private float gridDiff = 30;

    private void Awake()
    {
        allGridRoot = transform;
        allGridPrefab = new Dictionary<GridType, GameObject>();
        //��ʼ��
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
    /// ��������Ѿ�ƥ�����
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


        //ȷ�����Ž�������ƥ��ĸ��ӵ���������
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

        //ɾ��
        foreach (var items in matchTiles)
        {
            if (items == null)
                continue;
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                tiles[item.xIndex, item.yIndex] = null;

                //��ӷ���
                //AddScore

                Destroy(item.gameObject);
            }
        }

        //�ȴ�һ��ʱ��ȷ���������
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
                tileAbove.SetState(Tile.TileState.Moving); // �����Ϸ��ķ���
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
            //�ո�������ͳ��
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
                    //�п�λ �ƶ������߼�
                    tiles[i, j - emptySpaceCount] = currTile;
                    tiles[i, j] = null;
                    currTile.Init(i, j - emptySpaceCount, currTile.gridType);
                    currTile.StartFalling(j - emptySpaceCount, emptySpaceCount);
                }
            }


            //yield return new WaitForSeconds(0.2f);
            //���ݿո����������µķ���
            for (int t = 1; t <= emptySpaceCount; t++)
            {
                //�����·���
                CreateTileAtTop(i, t, emptySpaceCount);
            }
        }

        // �ȴ����з����������
        yield return new WaitForSeconds(0.5f); // �������䶯��ʱ������
    }


    /// <summary>
    /// �����¸��� ���ó�ʼλ�úͳ�ʼ��
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
        tiles[x, Column+1- fallDistance+ startY-1] = tile;//Column+1 ����������һ�� ��ȥ�ո������� ���ϲ�ͬλ��startY ��Ϊ�������±�������-1
        tile.StartFalling(Column + 1 - fallDistance + startY, fallDistance);//�·��鿪ʼ����
    }

    public void CheckForMatchesAt(int x, int y)
    {
        Tile currentTile = tiles[x, y];
        if (currentTile == null) return;
        //Debug.LogError("����֮��ļ��ƥ��");
        List<List<Tile>> matchedTiles = CheckForRemovableRegion();
        //Debug.LogError("------CheckForMatchesAt-----1");

        if (matchedTiles.Count == 0)
        {
            //Debug.LogError("----û��ƥ�������----");
            //û��ƥ��ĸ���֮����ȥ���� 

            return;
        }
        //Debug.LogError("------CheckForMatchesAt-----2");

        StartCoroutine(ClearAllMatchGrid(matchedTiles));
    }




    private List<List<Tile>> removableRegions;

    public static float tileSize = 1.024f;

    /// <summary>
    /// ������и����ж��Ƿ��п�������
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
                        //Debug.LogError("------------�п�����");
                        removableRegions.Add(regions);
                    }
                }
            }
        }
        return removableRegions;
    }

    public float padding = 10f; // ������֮��ļ��
    public float horizontalMargin = 50f; // Ԥ�������ұ߾�

    [HideInInspector]
    public float gridItemWidth;
    [HideInInspector]
    public float gridItemHeight;
    [HideInInspector]
    public float totalGridWidth;
    [HideInInspector]
    public float totalGridHeight;
    /// <summary>
    /// �������пո��� Ҳ���Ǳ�����
    /// </summary>
    public void CreatAllEmptyGrid()
    {
        Array.Clear(BGtiles, 0, BGtiles.Length);

        RectTransform canvasRect = transform.parent as RectTransform;
        float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        gridItemWidth = (availableWidth - padding * (Column - 1)) / Column;
        gridItemHeight = gridItemWidth; // ���������α���

        // �������������Ĵ�С
        RectTransform gridContainer = GetComponent<RectTransform>();
        totalGridWidth = Column * gridItemWidth + (Column - 1) * padding;
        //totalGridHeight = Row * gridItemHeight + (Row - 1) * padding;

        Debug.Log("��ǰ��Ļ�߶�Ϊ��" + UnityEngine.Screen.height);
        totalGridHeight = UnityEngine.Screen.height;
        gridContainer.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        // ��������������λ�ã�ʹ������Ļ�·�����
        gridContainer.anchorMin = new Vector2(0.5f, 0f);
        gridContainer.anchorMax = new Vector2(0.5f, 0f);
        gridContainer.pivot = new Vector2(0.5f, 0f);
        gridContainer.anchoredPosition = new Vector2(0f, 0f); // �ײ�����


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


    float fullScreenSpawnChance = 0.5f; // 50% �ĸ��� ȫ��ͬһ���ͼ���
    //float fullScreenSpawnChance = 0.05f; // 5% �ĸ��� ȫ��ͬһ���ͼ���

    /// <summary>
    /// ������Ϸ��Ʒ �ڸ��ӱ����ϵ�
    /// </summary>
    public void CreateCell()
    {
        GameObject obj;
        GridType curGridType=GridType.Null;

        bool isFullScreen = false;//�Ƿ�ȫ��ͬһ����

        if (UnityEngine.Random.value < fullScreenSpawnChance)
        {
            // ����ȫ��ͬ���������߼�
            curGridType = (GridType)GetRandomTile();
            isFullScreen =true;
        }
        else
        {
            // ������������߼�
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
    /// �����ų�˫��˫��  һ�Ӷ�
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


    private bool[,] visited; // ���ʱ������

    // �������������DFS������ͳ����������Ĵ�С
    private int DFS(int row, int col, GridType tileType, List<Tile> regon)
    {
        // ���߽��Tile�����Ƿ�ƥ��
        if (row < 0 || row >= Row || col < 0 || col >= Column || tiles[row, col] == null || visited[row, col] || tiles[row, col].gridType != tileType)
        {
            return 0;
        }

        visited[row, col] = true;

        regon.Add(tiles[row, col]);

        // ���ĸ�����ݹ����
        int count = 1;
        count += DFS(row - 1, col, tileType, regon); // ��
        count += DFS(row + 1, col, tileType, regon); // ��
        count += DFS(row, col - 1, tileType, regon); // ��
        count += DFS(row, col + 1, tileType, regon); // ��

        return count;
    }


    /// <summary>
    /// ��Ʒ�����������
    /// </summary>
    public int tileCount = 5;

    /// <summary>
    /// �����ͬ���͸���
    /// </summary>
    /// <returns></returns>
    int GetRandomTile()
    {
        int curTileType = UnityEngine.Random.Range(0, tileCount);
        return curTileType;
    }

    
    /// <summary>
    /// ������б�����
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
    /// ����λ��ȥ�������Ҫ���ֵ�λ�� ��  ��  ���
    /// </summary>
    /// <param name="x">��</param>
    /// <param name="y">��</param>
    /// <returns></returns>
    public Vector3 CorrectPositon(int x, int y)
    {
        //Debug.LogError($"��ǰ x :{x} y : {y} λ��x��{pos.x} λ��y: {pos.y}");
        return new Vector3(transform.position.x - (Row * gridWidth) / 2 + (x * gridWidth), transform.position.y - (Column * gridHeight) / 2 + (y * gridHeight));
    }

    /// <summary>
    /// ���ⲿ��ȡ���ӷ���
    /// </summary>
    /// <returns></returns>
    public GameObject GetGrid(GridType GridType, GameObject obj)
    {
        //TODO �����
        GameObject grid = ObjectPool.Instance.GetObject(GridType);
        if (!grid)
        {
            return grid;
        }
        return CreatGrid(GridType);
    }
    /// <summary>
    /// ���صĸ���Ԥ�� ��������clone
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
            Debug.LogError($"�������Ѿ���ʼ�����벻Ҫ�ظ���ʼ�� type:{gridType} path:{path}");
        }
        else
        {
            Debug.Log($"��ʼ���ɹ� type:{gridType} path:{path}");

            allGridPrefab.Add(gridType, gridPrefab);
        }
    }

    /// <summary>
    /// ����һ������
    /// </summary>
    private GameObject CreatGrid(GridType GridType)
    {
        if (allGridPrefab.ContainsKey(GridType))
        {
            GameObject gridPrefab = allGridPrefab[GridType];
            GameObject grid = GameObject.Instantiate(gridPrefab, allGridRoot);
            return grid;
        }
        Debug.LogError("�ֵ���û�м���Ԥ�ƣ����ʼ��");
        return null;
    }

    /// <summary>
    /// �����ɾ�� �ӽ������غ� �ŵ�����ع�ѭ��ʹ�� //TODO ������Ҫ���Ƿ�ŵ������ڵ���
    /// </summary>
    public void DestroyGrid(GridType GridType, GameObject obj)
    {
        obj.SetActive(false);
        ObjectPool.Instance.PutObject(GridType, obj);
    }

}
