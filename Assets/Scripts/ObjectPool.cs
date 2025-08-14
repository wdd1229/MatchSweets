using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GridType
{
    Blue,
    Yellow,
    Green,
    Red,
    Orange,
    Empty,
    Null//传入Null可创建空格子也就是背景格
}
/// <summary>
/// 对象池
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;

    public Dictionary<GridType,List<GameObject>> gridPrefabDict;

    //在使用时判断是否为空 否则创建预制，挂在脚本
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(ObjectPool).Name);
                instance = obj.AddComponent<ObjectPool>();
                DontDestroyOnLoad(obj); // 保证跨场景存在
            }
            return instance;
        }
    }

    private void Awake()
    {
        gridPrefabDict=new Dictionary<GridType,List<GameObject>>();
    }

    /// <summary>
    /// 从对象池中获取对象使用
    /// </summary>
    /// <param name="GridType"></param>
    /// <returns></returns>
    public GameObject GetObject(GridType GridType)
    {
        GameObject obj;
        if (gridPrefabDict.ContainsKey(GridType))
        {
            int idx = gridPrefabDict[GridType].Count-1;
            obj=gridPrefabDict[GridType][idx];
            gridPrefabDict[GridType].Remove(obj);
        }
        else
        {
            obj=null;
            Debug.Log($"当前类型:{GridType} 没有被提前添加类型，请检查初始化");
        }
        return obj;
    }

    /// <summary>
    /// 将资源放到对象池中
    /// </summary>
    /// <param name="GridType"></param>
    /// <param name="obj"></param>
    public void PutObject(GridType GridType, GameObject obj) 
    {
        if (gridPrefabDict.ContainsKey(GridType))
        {
            gridPrefabDict[GridType].Add(obj);
        }
    }
}
