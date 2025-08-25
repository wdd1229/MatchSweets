using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏所有得分数据
/// </summary>
public class GameScoreManager : Singleton<GameScoreManager>
{
    private AllScoreData allScoreData;
    public void InitInfo(AllScoreData allScoreData)
    {
        this.allScoreData = allScoreData;
    }

    /// <summary>
    /// 根据关卡、类型、和连接数量 获取到应得分数
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="connectCount"></param>
    public int GetScore(int levelIndex,GridType gridType,int connectCount)
    {
        if (allScoreData == null || allScoreData.scoreDatas.Count==0) 
            return 0;
        foreach (ScoreData scoreData in allScoreData.scoreDatas)
        {
            if(scoreData.curLevelIndex == levelIndex)
            {
                foreach (LevelScoreData levelScoreData in scoreData.levelScoreDatas)
                {
                    if(gridType== levelScoreData.gridType && connectCount== levelScoreData.connectCount)
                        return levelScoreData.score;
                }
            }
        }
        Debug.LogError($"没找到对于的分数，请检查json及逻辑 levelIndex: {levelIndex} gridType:{gridType} connectCount:{connectCount} ---  ");
        return 0;
    }
}
