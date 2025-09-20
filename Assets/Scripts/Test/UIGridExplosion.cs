// 网格控制器
using System.Collections.Generic;
using UnityEngine;

public class UIGridExplosion : MonoBehaviour
{

    private List<TileTest> tiles = new List<TileTest>();

    void Start()
    {
        GenerateGrid();
    }

    public Transform root;

    // 生成网格
    private void GenerateGrid()
    {
        //GetComponent().constraintCount = columns;

        //for (int i = 0; i & lt; rows * columns; i++)
        //{
        //    GameObject tileObj = Instantiate(tilePrefab, transform);
        //    TileTest tile = tileObj.AddComponent();
        //    tile.Initialize(this);
        //    tiles.Add(tile);
        //}

        for (int i = 0; i < root.childCount; i++)
        {
            GameObject obj = root.GetChild(i).gameObject;
            TileTest tile=obj.GetComponent<TileTest>();
            tiles.Add(tile);
            //tile.Initialize(this);
        }
    }

    // 触发爆炸
    [ContextMenu("触发爆炸")]
    public void TriggerExplosion()
    {
        foreach (TileTest tile in tiles)
        {
            Vector2 randomDir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            tile.StartExplosion(randomDir);
        }
    }
    [ContextMenu("触发爆炸22")]

    public void TriggerExplosion22()
    {
        foreach (TileTest tile in tiles)
        {
            // 随机方向（偏向上方）
            Vector2 dir = new Vector2(
                Random.Range(-0.8f, 0.8f),
                Random.Range(0.5f, 1f)
            );
            tile.StartExplosion(dir);
        }
    }
}
