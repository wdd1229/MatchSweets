using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int rows = 4; // 行数
    public int cols = 4; // 列数

    void Start()
    {
        int width = 9;
        int row = 4;
        float startX = (width - row) / 2.0f;
        Debug.LogError(startX); // 输出 2.5
        //Debug.LogError(GridSystem.Instance.WorldToGrid(new Vector3(300, 300)));



        //Debug.LogError(GridSystem.Instance.GridToWorld(new Vector3(2.5f, 0.5f)));


        //Debug.LogError(GridSystem.Instance.GridToWorld(new Vector3(5.5f, 0.5f)));


        GridSystem.Instance.GenerateGrid(6, 6);
    }



}
