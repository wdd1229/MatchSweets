using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 关卡管理
/// </summary>
public class GameLevelManager : Singleton<GameLevelManager>
{
    /// <summary>
    /// 当前关卡数据
    /// </summary>
    public LevelData curLevelData;

    private LevelList levelList;

    public int curLevelIndex=0;
    public void InitInfo(LevelList levelList)
    {
        Debug.Log(levelList);

        this.levelList = levelList;

        curLevelData=GetCurLevel_Index(curLevelIndex);
    }

    /// <summary>
    /// 获取当前关卡数据 根据index
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    private LevelData GetCurLevel_Index(int levelIndex)
    {
        if (levelList == null || this.levelList.levels.Length==0 || levelIndex >= this.levelList.levels.Length)
            return null;
        return this.levelList.levels[levelIndex];
    }

    public int GetCurLevelIndex()
    {
        return curLevelIndex;
    }

    public bool IsNextLevelCheck(int num)
    {
        if(curLevelData==null)
            return false;
        if(num>= curLevelData.wallCount)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 外部获取关卡数据
    /// </summary>
    /// <returns></returns>
    public LevelData GetCurLevel()
    {
        return this.curLevelData;
    }

    public bool NextLevel()
    {
        curLevelIndex += 1;
        curLevelData = GetCurLevel_Index(curLevelIndex);
        if (curLevelData == null)
        {
            Debug.LogError("游戏结束");
            return false;
        }
        else
        {
            Debug.LogError("获取下一关的数据成功");
            return true;
        }
    }


}
