using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GameManager : Singleton<GameManager>
{

    //public LevelList levelList;

    //public int curLevelIndex=0;

    private GridManager gridManager;

    private WallManager wallManager;

    private LevelPopupUI levelPopupUI;

    /// <summary>
    /// 当前收集品数量
    /// </summary>
    private int curSpacielCount = 0;

    /// <summary>
    /// 当前分数
    /// </summary>
    private int curScore = 0;

    private GameUi gameUi;

    private Transform Canvas;

    //private LevelData curLevelData;

    protected override void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        gridManager = Canvas.Find("GameUI/allGridRoot").GetComponent<GridManager>();

        wallManager = Canvas.Find("GameUI/walls").GetComponent<WallManager>();

        levelPopupUI = Canvas.Find("LevelPopupUI").GetComponent<LevelPopupUI>();

        gameUi = Canvas.Find("GameUI").GetComponent<GameUi>();

        LoadLevelData();

        ReadScoreData();

        levelPopupUI.Init();
    }
    public void GameStart()
    {
        gridManager.GameInit(GameLevelManager.Instance.GetCurLevel());
        wallManager.CreatWallOfLevelData(GameLevelManager.Instance.GetCurLevel().wallCount);
    }

    public void LoadLevelData()
    {
        GameLevelManager.Instance.InitInfo(LoadJson<LevelList>.LoadJsonFromFile("LevelData"));
    }

    public AllScoreData allScoreData;
    [ContextMenu("ScoreData文件读取")]
    public void ReadScoreData()
    {
         allScoreData = LoadJson<AllScoreData>.LoadJsonFromFile("ScoreData");
        GameScoreManager.Instance.InitInfo(allScoreData);
    }

    [ContextMenu("ScoreData文件写入")]
    public void SaveScoreDataList()
    {
        AllScoreData allScoreData = new AllScoreData();
        allScoreData.scoreDatas = new List<ScoreData>();

        ScoreData scoreDataList = new ScoreData();
        scoreDataList.curLevelIndex = 0;
        scoreDataList.levelScoreDatas = new List<LevelScoreData>();
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 3, 1));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,4,2));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,5,4));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,6,5));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,7,8));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,8,10));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,9,20));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,10,30));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,11,50));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,12,100));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,13,200));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Blue,14,400));

        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 3, 1));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 4, 4));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 5, 5));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 6, 10));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 7, 20));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 8, 30));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 9, 50));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 10, 100));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 11, 250));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 12, 500));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 13, 750));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 14, 800));

        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 3, 1));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 4, 5));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 5, 10));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 6, 20));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 7, 40));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 8, 80));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 9, 160));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 10, 500));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 11, 1000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 12, 2000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 13, 5000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 14, 6000));

        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 3, 1));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 4, 10));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 5, 30));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 6, 50));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 7, 60));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 8, 100));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 9, 750));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 10, 1000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 11, 10000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 12, 20000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 13, 50000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 14, 60000));

        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 3, 1));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 4, 20));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 5, 50));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 6, 100));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 7, 500));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 8, 1000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 9, 2000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 10, 5000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 11, 20000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 12, 50000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 13, 60000));
        scoreDataList.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 14, 80000));

        allScoreData.scoreDatas.Add(scoreDataList);


        ScoreData scoreDataList2 = new ScoreData();
        scoreDataList2.curLevelIndex = 1;
        scoreDataList2.levelScoreDatas = new List<LevelScoreData>();
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 5, 2));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 6, 4));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 7, 5));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 8, 8));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 9, 10));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 10, 20));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 11, 30));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 12, 50));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 13, 100));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 14, 200));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 15, 450));

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 5, 4));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 6, 5));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 7, 10));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 8, 20));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 9, 30));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 10, 50));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 11, 100));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 12, 250));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 13, 500));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 14, 750));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 15, 1000));

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 5, 5));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 6, 10));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 7, 20));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 8, 40));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 9, 80));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 10, 160));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 11, 500));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 12, 1000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 13, 2000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 14, 5000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 15, 7000));

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 5, 10));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 6, 30));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 7, 50));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 8, 60));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 9, 100));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 10, 750));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 11, 1000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 12, 10000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 13, 20000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 14, 50000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 15, 70000));

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 5, 20));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 6, 50));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 7, 100));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 8, 500));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 9, 1000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 10, 2000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 11, 5000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 12, 20000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 13, 50000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 14, 80000));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 15, 100000));

        allScoreData.scoreDatas.Add(scoreDataList2);


        ScoreData scoreDataList3 = new ScoreData();
        scoreDataList3.curLevelIndex = 2;
        scoreDataList3.levelScoreDatas = new List<LevelScoreData>();
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 6, 2));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 7, 4));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 8, 5));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 9, 8));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 10, 10));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 11, 20));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 12, 30));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 13, 50));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 14, 100));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 15, 200));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 16, 500));

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 6, 4));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 7, 5));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 8, 10));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 9, 20));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 10, 30));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 11, 50));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 12, 100));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 13, 250));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 14, 500));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 15, 750));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 16, 1200));

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 6, 5));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 7, 10));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 8, 20));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 9, 40));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 10, 80));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 11, 160));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 12, 500));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 13, 1000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 14, 2000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 15, 5000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 16, 8000));

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 6, 10));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 7, 30));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 8, 50));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 9, 60));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 10, 100));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 11, 750));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 12, 1000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 13, 10000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 14, 20000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 15, 50000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 16, 80000));

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 6, 20));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 7, 50));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 8, 100));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 9, 500));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 10, 1000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 11, 2000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 12, 5000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 13, 20000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 14, 50000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 15, 100000));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 16, 100000));

        allScoreData.scoreDatas.Add(scoreDataList3);

        LoadJson<AllScoreData>.SaveJsonToFile("ScoreData", allScoreData);
    }


    //AddSpecial
    public void RefreshSpecial()
    {
        curSpacielCount += 1;
        //ui刷新
        gameUi.RefreshSpecial(curSpacielCount);

        //墙壁减少
        wallManager.DesTroyWall();

        //if (GameLevelManager.Instance.IsNextLevelCheck(curSpacielCount))
        //{
        //    Debug.LogError("下一关");
        //    GameReset();
        //}
    }

    /// <summary>
    /// 获取分数根据传入类型连接数量
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="connectNum"></param>
    public void RefreshScore(GridType gridType,int connectNum)
    {
        if (gridType == GridType.SpecialCollection)
            return;
        int score=GameScoreManager.Instance.GetScore(GameLevelManager.Instance.GetCurLevelIndex(), gridType, connectNum);
        //Debug.LogError($"当前类型:{gridType} 当前连接数：{connectNum} 获得分数：{score}");
        curScore += score;
        gameUi.RefereshScore(curScore);

    }

    /// <summary>
    /// 展示弹窗UI
    /// </summary>
    public void ShowLevelPopup()
    {
        Debug.LogError($"当前收集品数量：{curSpacielCount} 当前分数：{curScore}");

        levelPopupUI.RefreshUI(curSpacielCount,curScore);
    }



    public bool IsNextLevelCheck()
    {
        return GameLevelManager.Instance.IsNextLevelCheck(curSpacielCount);
    }

    public void GameReset()
    {
        ResetSpecial();

        GameLevelManager.Instance.NextLevel();

        gridManager.GameReset(GameLevelManager.Instance.GetCurLevel());

        Debug.LogError(GameLevelManager.Instance.GetCurLevel());
        //gridManager.GameInit(GameLevelManager.Instance.GetCurLevel());
        wallManager.CreatWallOfLevelData(GameLevelManager.Instance.GetCurLevel().wallCount);

        //wallManager.DestroyAllWall();

        //执行其他操作 弹窗等

        //GameStart();
    }

    private void ResetSpecial()
    {
        curSpacielCount = 0;

        //ui刷新
        gameUi.RefreshSpecial(curSpacielCount);
    }
}
