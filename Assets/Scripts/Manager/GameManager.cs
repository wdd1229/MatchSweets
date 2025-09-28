using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{

    //public LevelList levelList;

    //public int curLevelIndex=0;
    [HideInInspector]
    public GridManager gridManager;

    private WallManager wallManager;

    private LevelPopupUI levelPopupUI;

    private bool aiState=false;

    /// <summary>
    /// 当前收集品数量
    /// </summary>
    private int curSpacielCount = 0;

    /// <summary>
    /// 当前分数
    /// </summary>
    private int curScore = 0;

    [HideInInspector]
    public GameUi gameUi;

    private Transform Canvas;


    private RewardUI rewardUI;


    //private LevelData curLevelData;

    private Transform floatingRoot;
    IEnumerator LoadLevelData()
    {
        // 只需传入相对路径
        string relativePath = "Data/LevelData.json";
        yield return StartCoroutine(JsonLoader.Instance.LoadJsonData<LevelList>(
            relativePath,
            SuccessLoadLevel,
           Error
        ));
    }

    IEnumerator LoadScoreData()
    {
        // 只需传入相对路径
        string relativePath = "Data/ScoreData.json";

        yield return StartCoroutine(JsonLoader.Instance.LoadJsonData<AllScoreData>(
            relativePath,
            SuccessLoadScore,
           Error
        ));
    }

    private void SuccessLoadLevel(LevelList levelList)
    {
        GameLevelManager.Instance.InitInfo(levelList);
    }
    private void SuccessLoadScore(AllScoreData allScoreData)
    {
        GameScoreManager.Instance.InitInfo(allScoreData);
    }

    private void Error(string msg)
    {
        Debug.LogError(msg);
    }


    protected override void Awake()
    {



        //TestLoadJson();



        Canvas = GameObject.Find("Canvas").transform;

        gridManager = Canvas.Find("GameUI/allGridRoot").GetComponent<GridManager>();

        wallManager = Canvas.Find("GameUI/walls").GetComponent<WallManager>();

        levelPopupUI = Canvas.Find("LevelPopupUI").GetComponent<LevelPopupUI>();

        gameUi = Canvas.Find("GameUI").GetComponent<GameUi>();
        floatingRoot = gameUi.transform.Find("floatingRoot");

        rewardUI = Canvas.Find("RewardUI").GetComponent<RewardUI>();

        //LoadLevelData();

        //ReadScoreData();

        levelPopupUI.Init();
    }

    private void Start()
    {
        StartCoroutine(LoadLevelData());
        StartCoroutine(LoadScoreData());
    }

    public void GameStart()
    {
        gridManager.GameInit(GameLevelManager.Instance.GetCurLevel());
        wallManager.CreatWallOfLevelData(GameLevelManager.Instance.GetCurLevel().wallCount);
    }

    //public void LoadLevelData()
    //{
    //    GameLevelManager.Instance.InitInfo(LoadJson<LevelList>.LoadJsonFromFile("LevelData"));
    //}

    [ContextMenu("ScoreData文件读取")]
    public void ReadScoreData()
    {
        StartCoroutine(LoadScoreData());
        // allScoreData = LoadJson<AllScoreData>.LoadJsonFromFile("ScoreData");
        //GameScoreManager.Instance.InitInfo(allScoreData);
    }

    [ContextMenu("ScoreData文件写入")]
    public void SaveScoreDataList()
    {
        AllScoreData allScoreData = new AllScoreData();
        allScoreData.scoreDatas = new List<ScoreData>();
        #region
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
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 3, 1));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 4, 1));
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

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 3, 1));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 4, 2));
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

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 3, 2));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 4, 3));
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

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 3, 3));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 4, 4));
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

        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 3, 6));
        scoreDataList2.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 4, 8));
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
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 3, 1));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 4, 1));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Blue, 5, 2));
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

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 3, 1));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 4, 1));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Yellow, 5, 2));
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

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 3, 1));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 4, 2));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Green, 5, 3));
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

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 3, 2));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 4, 4));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Red, 5, 6));
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

        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 3, 4));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 4, 8));
        scoreDataList3.levelScoreDatas.Add(new LevelScoreData(GridType.Orange, 5, 12));
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
        #endregion
        //JsonLoader.Instance.SaveJsonToFile("Data/ScoreData.json", allScoreData);
    }


    //AddSpecial
    public void RefreshSpecial()
    {
        //(PrefabManager.Instance.InstantiatefloatingPrefab(1,"floatingScore", floatingRoot, Vector3.zero)).transform.localPosition=new Vector3(0,0,0);
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
    public int RefreshScore(GridType gridType,int connectNum)
    {
        if (gridType == GridType.SpecialCollection)
            return 0;
        int score=GameScoreManager.Instance.GetScore(GameLevelManager.Instance.GetCurLevelIndex(), gridType, connectNum);
        //Debug.LogError($"当前类型:{gridType} 当前连接数：{connectNum} 获得分数：{score}");
        curScore += score;
        gameUi.RefereshScore(curScore);
        return score;
    }

    /// <summary>
    /// 展示弹窗UI
    /// </summary>
    public void ShowLevelPopup()
    {
        Debug.LogError($"当前收集品数量：{curSpacielCount} 当前分数：{curScore}");

        levelPopupUI.RefreshUI(curSpacielCount,curScore);
    }

    public void ShowReward()
    {
        //关卡结束进入奖励阶段
        StartCoroutine(gridManager.ClearAllEmptyGrid(null));


        gameUi.gameObject.SetActive(false);

        rewardUI.gameObject.SetActive(true);
    }


    public bool IsNextLevelCheck()
    {
        return GameLevelManager.Instance.IsNextLevelCheck(curSpacielCount);
    }

    public void GameReset()
    {
        ResetSpecial();

        if (GameLevelManager.Instance.NextLevel() == false)
        {
            ////关卡结束进入奖励阶段
            //StartCoroutine(gridManager.ClearAllEmptyGrid(null));

            return;
        }


        Debug.LogError(GameLevelManager.Instance.GetCurLevel().ToString());

        //gridManager.GameInit(GameLevelManager.Instance.GetCurLevel());
        wallManager.CreatWallOfLevelData(GameLevelManager.Instance.GetCurLevel().wallCount);

        StartCoroutine(gridManager.GameReset(GameLevelManager.Instance.GetCurLevel()));

    }

    private void ResetSpecial()
    {
        curSpacielCount = 0;

        //ui刷新
        gameUi.RefreshSpecial(curSpacielCount);
    }

    public void ShowRewardTip(string msg)
    {
        rewardUI.ShowRewardTip(msg);
    }


    //private void TestVibrate()
    //{
    //    long[] pattern = { 0, 100, 1000, 300 };
    //    TT.Vibrate(pattern);
    //}

    //private void TestVibrateShort()
    //{
    //    long[] pattern = { 400 };
    //    TT.Vibrate(pattern);
    //}

    public void ResetGrid()
    {
        StartCoroutine(ResetGridCoroutine());
    }

    IEnumerator ResetGridCoroutine()
    {
        Debug.LogError("ResetGrid");
        
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(gridManager.GameReset(GameLevelManager.Instance.GetCurLevel()));
    }
    public void SetAiState(bool state)
    {
        aiState = state;
    }

    public bool GetAiState()
    {
        return aiState;
    }
}
