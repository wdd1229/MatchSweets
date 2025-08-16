using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int rows = 4; // ����
    public int cols = 4; // ����
    public Vector2 cellSize = new Vector2(1.024f, 1.024f); // ÿ�����ӵĴ�С
    public RectTransform gridPrefab; // ������� RectTransform
    public float padding = 10f; // ������֮��ļ��
    public float horizontalMargin = 50f; // Ԥ�������ұ߾�

    void Start()
    {
        RectTransform canvasRect = transform.parent as RectTransform;
        float availableWidth = canvasRect.rect.width - 2 * horizontalMargin;
        float gridItemWidth = (availableWidth - padding * (cols - 1)) / cols;
        float gridItemHeight = gridItemWidth; // ���������α���

        // �������������Ĵ�С
        RectTransform gridContainer = GetComponent<RectTransform>();
        float totalGridWidth = cols * gridItemWidth + (cols - 1) * padding;
        float totalGridHeight = rows * gridItemHeight + (rows - 1) * padding;
        gridContainer.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        // ��������������λ�ã�ʹ������Ļ�·�����
        gridContainer.anchorMin = new Vector2(0.5f, 0f);
        gridContainer.anchorMax = new Vector2(0.5f, 0f);
        gridContainer.pivot = new Vector2(0.5f, 0f);
        gridContainer.anchoredPosition = new Vector2(0f, 0f); // �ײ�����

        // ��̬����������
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
}
