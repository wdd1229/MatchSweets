using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum ObjectType { Grid, floatingScore }
/// <summary>
/// 对象池管理类，支持任意预制体（Prefab）的复用。
/// </summary>
public class ObjectPooler : Singleton<ObjectPooler>
{
    public Transform allroot;

    public Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
    private int defaultPoolSize = 15;



    protected override void Awake()
    {
        base.Awake();
        allroot = GameObject.Find("Canvas/GameUI/allGridRoot").transform;
        Debug.LogError(allroot);
    }

    // 初始化池
    public void Initialize(string prefabName, ObjectType objectType,int poolSize = 15)
    {
        if (prefabName == null || string.IsNullOrEmpty(prefabName))
            return;
        if (objectPools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"Object pool for '{prefabName}' already initialized.");
            return;
        }
        GameObject prefab = Resources.Load<GameObject>(objectType.ToString()+"/"+prefabName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab '{prefabName}' not found in Resources. Make sure you've placed it in the correct folder.");
            return;
        }
        objectPools[prefabName] = new Queue<GameObject>();
        Debug.LogError($"prefabName:{prefabName}");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, allroot);
            obj.SetActive(false);
            objectPools[prefabName].Enqueue(obj);
        }
        Debug.Log($"Object pool for '{prefabName}' initialized with size {poolSize}.");
    }
    // 从对象池中获取一个 GameObject
    public GameObject GetFromPool(string prefabName,ObjectType objectType)
    {
        if (!objectPools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"Object pool for '{prefabName}' is not initialized. Creating now.");
            Initialize(prefabName, objectType);
        }
        if (objectPools[prefabName].Count > 0)
        {
            GameObject obj = objectPools[prefabName].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        Debug.LogWarning($"No available objects in the pool for '{prefabName}'. Creating a new one.");
        return GameObject.Instantiate(Resources.Load<GameObject>(objectType.ToString() + "/" + prefabName),allroot);
    }
    // 归还到对象池
    public void ReturnToPool(string prefabName, GameObject obj)
    {
        if (obj == null || !objectPools.ContainsKey(prefabName))
            return;
        obj.SetActive(false);
        objectPools[prefabName].Enqueue(obj);
        //obj.transform.SetParent(transform);
    }
}
