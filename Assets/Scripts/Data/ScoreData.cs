using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AllScoreData
{
    public List<ScoreData> scoreDatas;
}
[Serializable]
public class ScoreData
{
    public int curLevelIndex;
    public List<LevelScoreData> levelScoreDatas;
}
[Serializable]
public class LevelScoreData 
{
    public GridType gridType;
    public int connectCount;
    public int score;

    public LevelScoreData(GridType gridType, int connectCount, int score)
    {
        this.gridType = gridType;
        this.connectCount = connectCount;
        this.score = score;
    }
}
