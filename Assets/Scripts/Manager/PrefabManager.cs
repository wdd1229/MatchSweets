using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum FloatingType { specialFloating, normalFloating }

/// <summary>
/// 预制控制 加载克隆
/// </summary>
public class PrefabManager : Singleton<PrefabManager>
{
    PrefabManifest gridPrefabManifest;

    PrefabManifest floatingPrefabManifest;


    // 已实例化的对象池
    private Dictionary<string, Queue<GameObject>> _objectPool =
        new Dictionary<string, Queue<GameObject>>();

    protected override  void Awake()
    {
        gridPrefabManifest = Resources.Load<PrefabManifest>("GuidPrefabs");
        floatingPrefabManifest = Resources.Load<PrefabManifest>("floatingScore");
    }

    public GameObject InstantiatefloatingPrefab(FloatingType floatingType,int score, Transform parent = null, Vector3? position = null, Quaternion? rotation = null)
    {
        GameObject prefab = floatingPrefabManifest.GetPrefab(floatingType.ToString());
        if (prefab == null) return null;

        // 尝试从对象池获取
        if (TryGetFromPool(floatingType.ToString(), out GameObject instance))
        {
            instance.transform.position = position ?? Vector3.zero;
            instance.transform.rotation = rotation ?? Quaternion.identity;
            instance.SetActive(true);
            instance.GetComponent<floatingScore>().SetScore(score);
            return instance;
        }

        Vector3 pos = position ?? Vector3.zero;
        // 创建新实例
        instance = Instantiate(prefab, position ?? Vector3.zero, rotation ?? Quaternion.identity, parent);
        instance.GetComponent<floatingScore>().SetScore(score);
        instance.name = $"{prefab.name}_Instance";
        return instance;
    }

    public GameObject InstantiateGridPrefab(string name,Transform parent=null, Vector3? position=null,Quaternion? rotation = null)
    {
        GameObject prefab = gridPrefabManifest.GetPrefab(name);
        if (prefab == null) return null;

        // 尝试从对象池获取
        if (TryGetFromPool(name, out GameObject instance))
        {
            instance.transform.position = position ?? Vector3.zero;
            instance.transform.rotation = rotation ?? Quaternion.identity;
            instance.SetActive(true);
            return instance;
        }

        // 创建新实例
        instance = Instantiate(prefab, position ?? Vector3.zero, rotation ?? Quaternion.identity, parent);
        instance.name = $"{prefab.name}_Instance";
        return instance;
    }


    /// <summary>
    /// 回收实例
    /// </summary>
    public void RecycleInstance(GameObject instance)
    {
        if (instance == null) return;

        string key = instance.name.Replace("_Instance", "");

        if (!_objectPool.ContainsKey(key))
        {
            _objectPool[key] = new Queue<GameObject>();
        }

        instance.SetActive(false);
        _objectPool[key].Enqueue(instance);
    }

    

    // 尝试从对象池获取实例
    private bool TryGetFromPool(string prefabPath, out GameObject instance)
    {
        instance = null;

        if (_objectPool.TryGetValue(prefabPath, out Queue<GameObject> pool) && pool.Count > 0)
        {
            instance = pool.Dequeue();
            return true;
        }
        return false;
    }
}
