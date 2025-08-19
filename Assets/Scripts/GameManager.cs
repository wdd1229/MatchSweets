using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public LevelList levelList;

    public int curLevelIndex=0;

    private GridManager gridManager;

    private WallManager wallManager;

    private int curSpacielCount = 0;

    private GameUi gameUi;

    private Transform Canvas;

    private LevelData curLevelData;

    protected override void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        gridManager = Canvas.Find("GameUI/allGridRoot").GetComponent<GridManager>();

        wallManager = Canvas.Find("GameUI/walls").GetComponent<WallManager>();

        gameUi= Canvas.Find("GameUI").GetComponent<GameUi>();

        LoadLevelData();


    }
    public void GameStart()
    {
        curLevelData = levelList.levels[0];
        gridManager.GameInit(curLevelData);
        wallManager.CreatWallOfLevelData(curLevelData.wallCount);
    }
    
    public void LoadLevelData()
    {
        levelList = LoadJson<LevelList>.LoadJsonFromFile("LevelData");
        Debug.Log(levelList);
    }
    //AddSpecial
    public void RefreshSpecial()
    {
        curSpacielCount += 1;
        //ui刷新
        gameUi.RefreshSpecial(curSpacielCount);

        //墙壁减少
        wallManager.DesTroyWall();
    }
}
