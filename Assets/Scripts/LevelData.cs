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
    /// �ؿ�
    /// </summary>
    public int levelIndex;
    /// <summary>
    /// ����
    /// </summary>
    public int Row;
    /// <summary>
    /// ����
    /// </summary>
    public int Column;
    /// <summary>
    /// ǽ����
    /// </summary>
    public int wallCount;
}

public class ScoreData
{

}
