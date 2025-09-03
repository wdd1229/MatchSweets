using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : Singleton<GridSystem>
{
    [Header("网格参数")]
    public float cellSize = 1f;      // 网格单元大小
    public int width = 10;            // 网格宽度（格子数）
    public int height = 10;           // 网格高度（格子数）

    private Vector3 origin;           // 网格原点（左下角）

    void Start()
    {
        //width = Screen.width / 120;
        //height = Screen.height / 120;

        origin = transform.position;
        occupiedCells = new bool[width, height];
    }

    public void Setorigin(Vector3 origin)
    {
        this.origin = origin;
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

    // 检查网格坐标是否在范围内
    public bool IsValidPosition(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width &&
               gridPos.y >= 0 && gridPos.y < height;
    }

    public GameObject prefab;
    public void GenerateGrid(int row,int column)
    {
        //整体减去需要的网格宽度 /2计算出起始点
        float startX=(width - row)/2.0f;
        float startY=1f;

        for (int i = 0; i < row; i++)
        {
            startX = (width - row) / 2.0f;
            for (int j = 0; j < column; j++)
            {
                //Debug.LogError($"{startX}---{startY}");
                Vector3 pos = GridToWorld(new Vector2(startX, startY));
                GameObject item=GameObject.Instantiate(prefab, transform);
                item.transform.localPosition= pos;

                startX += 1;
            }
            startY += 1;
        }
        startX = (width - row) / 2.0f;
        startY = 14;
        for (int i = 0;i < column; i++)
        {
            Vector3 pos = GridToWorld(new Vector2(startX, startY));
            GameObject item = GameObject.Instantiate(prefab, transform);
            item.transform.localPosition = pos;
            startX += 1;
        }
    }



    private bool[,] occupiedCells; // 记录被占用的网格



    // 检查区域是否可用
    public bool IsAreaAvailable(Vector2Int gridPos, Vector2Int size)
    {
        for (int x = gridPos.x; x < gridPos.x + size.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + size.y; y++)
            {
                if (!IsValidPosition(new Vector2Int(x, y)) || occupiedCells[x, y])
                    return false;
            }
        }
        return true;
    }

    // 占用网格区域
    public void OccupyArea(Vector2Int gridPos, Vector2Int size)
    {
        for (int x = gridPos.x; x < gridPos.x + size.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + size.y; y++)
            {
                occupiedCells[x, y] = true;
            }
        }
    }



    void OnDrawGizmos()
    {
        if (!Application.isPlaying) origin = transform.position;

        Gizmos.color = Color.cyan;

        // 绘制水平线
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = origin + new Vector3(0, y * cellSize, 0);
            Vector3 end = start + new Vector3(width * cellSize, 0, 0);
            Debug.DrawLine(start, end,Color.red);
            Gizmos.DrawLine(start, end);
        }

        // 绘制垂直线
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, height * cellSize, 0);
            Gizmos.DrawLine(start, end);
            Debug.DrawLine(start, end,Color.red);

        }
    }
}
