using Unity.VisualScripting;
using UnityEngine;
public enum FloatingType { specialFloating, normalFloating };

public class PrefabManager : Singleton<PrefabManager>
{
    //private ObjectPooler objectPooler;

    public GameObjectsData floatingPrefabs;
    public GameObjectsData gridPrefabs;

    private static string gridPrefabPath = "Grid/";

    private Transform allroot;
    protected override void Awake()
    {
        allroot = transform.Find("Canvas/GameUI/allGridRoot");

        LoadGameObjectDatas();

        //objectPooler =new GameObject("ObjectPooler").AddComponent<ObjectPooler>();
        InitializeGridPools();//根据GameObjectsData 初始化池子

        InitializeFloatingScorePools();
    }

    private void LoadGameObjectDatas()
    {
        //加载资源
        floatingPrefabs = Resources.Load<GameObjectsData>("ScriptableObjects/floatingPrefabs");
        gridPrefabs = Resources.Load<GameObjectsData>("ScriptableObjects/gridPrefabs");

        gridPrefabs.Initialize();
        floatingPrefabs.Initialize();
    }

    private void InitializeGridPools()
    {
        if(gridPrefabs == null)
        {
            Debug.LogError("Grid prefab data not found in Resources Please make sure it's correctly placed");
            return;
        }
        foreach (string key in gridPrefabs.prefabDictionary.Keys)
        {
            int poolSize = key == "SpecialCollection" ? 1 : 10;//特殊收集品一次只会有一个
            ObjectPooler.Instance.Initialize(key, ObjectType.Grid,poolSize);
        }
    }

    private void InitializeFloatingScorePools()
    {
        if (floatingPrefabs == null) 
        {
            Debug.LogError("floating prefab data not found in Resources Please make sure it's correctly placed");
            return;
        }
        foreach (string key in floatingPrefabs.prefabDictionary.Keys)
        {
            ObjectPooler.Instance.Initialize(key, ObjectType.floatingScore, 1);
        }
    }

    public GameObject GetFloatingScorePrefab(FloatingType floatingType,int score, Transform parent)
    {
        GameObject obj = ObjectPooler.Instance.GetFromPool(floatingType.ToString(), ObjectType.floatingScore);
        if(obj == null)
        {
            Debug.LogError($"FloatingScorePrefab:{floatingType} not found in pool");
            return null;
        }
        if(obj.GetComponent<FloatingScore>() != null)
        {
            obj.GetComponent<FloatingScore>().Init(floatingType,score);
            obj.SetActive(true);
        }
        return obj;
    }

    public void ReturnFloatingScorePrefab(GameObject obj,FloatingType floatingType)
    {
        ObjectPooler.Instance.ReturnToPool(floatingType.ToString(), obj);
    }

    /// <summary>
    /// 获取格子预制体--从对象池中获取
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject GetGridPrefab(GridType gridType,Transform parent)
    {
        //string prefabName=gridType.ToString();
        GameObject obj= ObjectPooler.Instance.GetFromPool(gridType.ToString(), ObjectType.Grid);
        if (obj == null) 
        {
            Debug.LogError($"GridPrefab:{gridType} not found in pool");
            return null;
        }
        if(obj.GetComponent<Tile>()==null)
            obj.AddComponent<Tile>();
        return obj;
    }

    /// <summary>
    /// 将格子归还到对象池中
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gridType"></param>
    public void ReturnGridPrefab(GameObject obj,GridType gridType)
    {
        //string prefabName = gridType.ToString();
        if(obj.GetComponent<Tile>()!=null)
            obj.GetComponent<Tile>().ResetState();
        ObjectPooler.Instance.ReturnToPool(gridType.ToString(), obj);
    }

}
