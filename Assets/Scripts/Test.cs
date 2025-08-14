using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    //public GameObject gridPrefab; // 格子预制体
    //public int rows = 5; // 行数
    //public int cols = 5; // 列数
    //public Vector2 cellSize = new Vector2(1.024f, 1.024f); // 每个格子的大小

    //void Start()
    //{
    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < cols; j++)
    //        {
    //            GameObject cell = Instantiate(gridPrefab, transform);
    //            cell.transform.localPosition = new Vector3(j * cellSize.x, -i * cellSize.y, 0);
    //        }
    //    }
    //}

    public GameObject gridPrefab; // 格子预制体
    public int rows = 4; // 行数
    public int cols = 4; // 列数
    public Vector2 cellSize = new Vector2(1.024f, 1.024f); // 每个格子的大小

    void Start()
    {
        GenerateGrid();
        AlignGridToBottomCenter();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject cell = Instantiate(gridPrefab, transform);
                cell.transform.localPosition = new Vector3(j * cellSize.x, -i * cellSize.y, 0);
            }
        }
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
}
