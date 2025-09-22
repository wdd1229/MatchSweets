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
            Debug.LogError("*************");
            Debug.LogError(key == "SpacialCollection");
            Debug.LogError("*************");
            int poolSize = key == "SpacialCollection" ? 1 : 10;//特殊收集品一次只会有一个

            Debug.LogError(poolSize);
            ObjectPooler.Instance.Initialize(key, ObjectType.Grid,poolSize);
        }

        Debug.LogError("加载结束");
    }

    /// <summary>
    /// 获取格子预制体--从对象池中获取
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject GetGridPrefab(GridType gridType,Transform parent)
    {
        string prefabName=gridType.ToString();
        GameObject obj= ObjectPooler.Instance.GetFromPool(prefabName, ObjectType.Grid);
        if (obj == null) 
        {
            Debug.LogError($"GridPrefab:{prefabName} not found in pool");
            return null;
        }
        //obj.transform.SetParent(parent,false);
        //obj.transform.GetComponent<Tile>().SetState(Tile.TileState.Idle);
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
        string prefabName = gridType.ToString();
        obj.GetComponent<Tile>().ResetState();
        ObjectPooler.Instance.ReturnToPool(prefabName,obj);
    }

    public GameObject InstantiatefloatingPrefab(FloatingType floatingType,int score,Transform parent,Vector3 pos)
    {
        GameObject floatObj;
        //if (floatingPrefabs.ContainsKey(floatingType.ToString(), out floatObj))
        //{
        //    GameObject curPrefab=GameObject.Instantiate(floatObj,parent);
        //    //curPrefab.transform.SetParent(parent,true);
        //    curPrefab.GetComponent<FloatingScore>().Init(score);
        //    return curPrefab;
        //}

        return null;
    }

    public GameObject InstantiateGridPrefab(string guideName,Transform parent)
    {
        GameObject gridObj;
        //if (gridPrefabs.ContainsKey(guideName, out gridObj))
        //{
        //    GameObject curPrefab = GameObject.Instantiate(gridObj, parent);
        //    //curPrefab
        //    return curPrefab;
        //}
        return null;
    }
}
