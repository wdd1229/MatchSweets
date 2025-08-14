using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.UI.Image;


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
    public float gridWidth;
    /// <summary>
    /// ���Ӹ߶�
    /// </summary>
    public float gridHeight;

    /// <summary>
    /// ȫ���ķ���
    /// </summary>
    private Tile[,] tiles;


    private void Awake()
    {
        allGridRoot = transform;
        allGridPrefab =new Dictionary<GridType, GameObject> ();
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
        tiles=new Tile[Row, Column];

        CreatAllEmptyGrid();

        AlignGridToBottomCenter();

        CreateCell();

        CheckAllMatchGrid();
    }

    private float tileSize = 1.024f;

    /// <summary>
    /// �������пո��� Ҳ���Ǳ�����
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
    /// ������Ϸ��Ʒ �ڸ��ӱ����ϵ�
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
    /// �������ƥ�����
    /// </summary>
    public void CheckAllMatchGrid()
    {
        List<Tile> matchList=new List<Tile>();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                 matchList = MatchCalculate(tiles[i,j],i,j);

                //Debug.LogError($" �������� x:{matchList[t].xIndex} y:{matchList[t].yIndex} ");
            }
        }
        //if (matchList != null || matchList.Count>0) {
        //    for (int t = 0; t < matchList.Count; t++)
        //    {
        //        Debug.LogError($" �������� x:{matchList[t].xIndex} y:{matchList[t].yIndex} ");
        //    }
        //}
        
    }

    private List<Tile> MatchCalculate(Tile tile,int newX,int newY)
    {
        GridType curGridType = tile.gridType;
        List<Tile> matchRow = new List<Tile>();
        List<Tile> matchLine = new List<Tile>();
        List<Tile> finishedMatching = new List<Tile>();

        //��ƥ��
        matchRow.Add(tile);

        //i=0��������i=1��������
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

        //�м��
        if (matchRow.Count >= 3) 
        {
            for (int i = 0; i < matchRow.Count; i++)
            {
                //��ƥ���б�������ƥ��������ÿ��Ԫ������һ�ν����б������

                //0�����Ϸ� 1�����·�
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

        //��ƥ��
        //i=0��������i=1��������
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
    /// ��Ʒ�����������
    /// </summary>
    public int tileCount=5;

    int GetRandomTile()
    {
        int curTileType=Random.Range(0,tileCount);
        return curTileType;
    }

    void AlignGridToBottomCenter()
    {
        // ��ȡ���������Χ��
        Bounds bounds = CalculateGridBounds();

        // �����������ĵ����������
        Vector3 gridCenter = bounds.center;
        float gridWidth = bounds.size.x;
        float gridHeight = bounds.size.y;

        // ��ȡ������ӿڵײ����ĵ���������
        Camera mainCamera = Camera.main;
        float bottomY = mainCamera.ViewportToWorldPoint(Vector3.zero).y;
        float centerX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;

        // ����ƫ������ʹ����ײ�����
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
    /// ������б�����
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
    public GameObject GetGrid(GridType GridType,GameObject obj)
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
    private Dictionary<GridType,GameObject> allGridPrefab;

    private void AddPrefab(GridType gridType)
    {
        string path;
        GameObject gridPrefab;
        path = gridType.ToString() + "_Grid";
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
            GameObject gridPrefab= allGridPrefab[GridType];
            GameObject grid = GameObject.Instantiate(gridPrefab, allGridRoot);
            return grid;
        }
        Debug.LogError("�ֵ���û�м���Ԥ�ƣ����ʼ��");
        return null;
    }

    /// <summary>
    /// �����ɾ�� �ӽ������غ� �ŵ�����ع�ѭ��ʹ�� //TODO ������Ҫ���Ƿ�ŵ������ڵ���
    /// </summary>
    public void DestroyGrid(GridType GridType,GameObject obj) 
    {
        obj.SetActive(false);
        ObjectPool.Instance.PutObject(GridType, obj);
    }

}
