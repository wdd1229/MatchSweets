using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelList levelList;

    public int curLevelIndex=0;

    private GridManager gridManager;

    private WallManager wallManager;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();

        wallManager = GameObject.Find("Canvas/GameUI/walls").transform.GetComponent<WallManager>();
    }

    void Start()
    {
        LoadLevelData();
        gridManager.GameInit(levelList.levels[0]);
        wallManager.CreatWallOfLevelData(levelList.levels[0].wallCount);
    }
    
    public void LoadLevelData()
    {
        levelList = LoadJson<LevelList>.LoadJsonFromFile("LevelData");
        Debug.Log(levelList);
    }
}
