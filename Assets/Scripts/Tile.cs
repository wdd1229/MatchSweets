using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public GridType gridType;
    public void Init(int x,int y,GridType gridType)
    {
        if (gridType == GridType.Null || gridType == GridType.Empty)
            return;
        xIndex = x;
        yIndex = y;
        this.gridType = gridType;
        // ���÷��������Ϊ������
        name = "Tile (" + xIndex + ", " + yIndex + ")";
    }
}
