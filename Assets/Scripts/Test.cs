using System;
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
        //RectTransform canvasRect = transform.parent as RectTransform;
        //float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        //float gridItemWidth = (availableWidth - padding * (cols - 1)) / cols;
        //float gridItemHeight = gridItemWidth; // 保持正方形比例

        //// 设置网格容器的大小
        //RectTransform gridContainer = GetComponent<RectTransform>();
        //float totalGridWidth = cols * gridItemWidth + (cols - 1) * padding;
        //float totalGridHeight = rows * gridItemHeight + (rows - 1) * padding;
        //gridContainer.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        //// 设置网格容器的位置，使其在屏幕下方居中
        //gridContainer.anchorMin = new Vector2(0.5f, 0f);
        //gridContainer.anchorMax = new Vector2(0.5f, 0f);
        //gridContainer.pivot = new Vector2(0.5f, 0f);
        //gridContainer.anchoredPosition = new Vector2(0f, 0f); // 底部居中

        //// 动态生成网格项
        //for (int i = 0; i < cols; i++)
        //{
        //    for (int j = 0; j < rows; j++)
        //    {
        //        RectTransform grid = Instantiate(gridPrefab, transform);
        //        grid.sizeDelta = new Vector2(gridItemWidth, gridItemHeight);
        //        grid.anchoredPosition = new Vector2(
        //            -totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding),
        //            -totalGridHeight / 2 + gridItemHeight / 2 + j * (gridItemHeight + padding)
        //        );
        //    }
        //}

        CreatAllEmptyGrid();
    }

    public int Row = 4; // 行数
    public int Column = 4; // 列数

    private float gridItemWidth;
    private float gridItemHeight;
    private float totalGridWidth;
    private float totalGridHeight;
    public void CreatAllEmptyGrid()
    {

        RectTransform canvasRect = transform.parent as RectTransform;
        float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        gridItemWidth = (availableWidth - padding * (Column - 1)) / Column;
        gridItemHeight = gridItemWidth; // 保持正方形比例

        // 设置网格容器的大小
        RectTransform gridContainer = GetComponent<RectTransform>();
        totalGridWidth = Column * gridItemWidth + (Column - 1) * padding;
        //totalGridHeight = Row * gridItemHeight + (Row - 1) * padding;

        float canvasHeight = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.height;

        Debug.Log("当前屏幕高度为：" + UnityEngine.Screen.height);
        Debug.Log("当前Canvas高度为：" + canvasHeight);

        //RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        //float topPosition = canvasRect.rect.height * 0.5f; // ��ȡCanvas�߶ȵ�һ�룬������λ��
        Rect safeArea = UnityEngine.Screen.safeArea;
        float topInset = UnityEngine.Screen.height - safeArea.yMax;

        totalGridHeight = UnityEngine.Screen.height;
        //gridContainer.sizeDelta = new Vector2(totalGridWidth, UnityEngine.Screen.height);
        gridContainer.sizeDelta = new Vector2(totalGridWidth, canvasHeight);
        Debug.Log("当前屏幕高度****");
        // ��������������λ�ã�ʹ������Ļ�·�����
        gridContainer.anchorMin = new Vector2(0.5f, 0f);
        gridContainer.anchorMax = new Vector2(0.5f, 0f);
        gridContainer.pivot = new Vector2(0.5f, 0f);
        gridContainer.anchoredPosition = new Vector2(0f, 0f); // 底部居中


        GameObject obj;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column + 1; j++)
            {
                obj = GameObject.Instantiate(gridPrefab.gameObject,transform);

                RectTransform grid = obj.GetComponent<RectTransform>();
                grid.sizeDelta = new Vector2(gridItemWidth, gridItemHeight);

                Tile tile = obj.AddComponent<Tile>();

                if (j == Column)
                {
                    tile.Init(i, j, GridType.Top);

                    grid.localPosition = new Vector3(-totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding), canvasHeight - gridItemHeight / 2 - padding, 0);

                }
                else
                {

                    tile.Init(i, j, GridType.Empty);

                    grid.anchoredPosition = new Vector2(
                   -totalGridWidth / 2 + gridItemWidth / 2 + i * (gridItemWidth + padding),
                   -totalGridHeight / 2 + gridItemHeight / 2 + j * (gridItemHeight + padding)
                    );
                }


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

    
}
