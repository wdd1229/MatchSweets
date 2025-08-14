using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    //public GameObject gridPrefab; // ����Ԥ����
    //public int rows = 5; // ����
    //public int cols = 5; // ����
    //public Vector2 cellSize = new Vector2(1.024f, 1.024f); // ÿ�����ӵĴ�С

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

    public GameObject gridPrefab; // ����Ԥ����
    public int rows = 4; // ����
    public int cols = 4; // ����
    public Vector2 cellSize = new Vector2(1.024f, 1.024f); // ÿ�����ӵĴ�С

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
