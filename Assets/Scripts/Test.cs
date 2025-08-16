using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int rows = 4; // 行数
    public int cols = 4; // 列数
    public Vector2 cellSize = new Vector2(1.024f, 1.024f); // 每个格子的大小
    public RectTransform gridPrefab; // 网格项的 RectTransform
    public float padding = 10f; // 网格项之间的间距
    public float horizontalMargin = 50f; // 预留的左右边距

    void Start()
    {
        RectTransform canvasRect = transform.parent as RectTransform;
        float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        float gridItemWidth = (availableWidth - padding * (cols - 1)) / cols;
        float gridItemHeight = gridItemWidth; // 保持正方形比例

        // 设置网格容器的大小
        RectTransform gridContainer = GetComponent<RectTransform>();
        float totalGridWidth = cols * gridItemWidth + (cols - 1) * padding;
        float totalGridHeight = rows * gridItemHeight + (rows - 1) * padding;
        gridContainer.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        // 设置网格容器的位置，使其在屏幕下方居中
        gridContainer.anchorMin = new Vector2(0.5f, 0f);
        gridContainer.anchorMax = new Vector2(0.5f, 0f);
        gridContainer.pivot = new Vector2(0.5f, 0f);
        gridContainer.anchoredPosition = new Vector2(0f, 0f); // 底部居中

        // 动态生成网格项
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                RectTransform grid = Instantiate(gridPrefab, transform);
                grid.sizeDelta = new Vector2(gridItemWidth, gridItemHeight);
                grid.anchoredPosition = new Vector2(
                    -totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding),
                    -totalGridHeight / 2 + gridItemHeight / 2 + j * (gridItemHeight + padding)
                );
            }
        }
    }





    //void Start()
    //{
    //    GenerateGrid();
    //    AlignGridToBottomCenter();
    //}

    //void GenerateGrid()
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
