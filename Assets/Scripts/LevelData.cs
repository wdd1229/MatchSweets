using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelList
{
    public LevelData[] levels;
}
[Serializable]
public class LevelData 
{
    /// <summary>
    /// 关卡
    /// </summary>
    public int levelIndex;
    /// <summary>
    /// 行数
    /// </summary>
    public int Row;
    /// <summary>
    /// 列数
    /// </summary>
    public int Column;
    /// <summary>
    /// 墙壁数
    /// </summary>
    public int wallCount;
}

public class ScoreData
{

}
