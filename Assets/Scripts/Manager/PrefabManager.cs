using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
public enum FloatingType { specialFloating, normalFloating };

public class PrefabManager : Singleton<PrefabManager>
{
    public GameObjectsData floatingPrefabs;
    public GameObjectsData gridPrefabs;

    protected override void Awake()
    {
        floatingPrefabs = Resources.Load<GameObjectsData>("Test/floatingPrefabs");
        gridPrefabs = Resources.Load<GameObjectsData>("Test/gridPrefabs");
    }

    public GameObject InstantiatefloatingPrefab(FloatingType floatingType,int score,Transform parent,Vector3 pos)
    {
        GameObject floatObj;
        if (floatingPrefabs.ContainsKey(floatingType.ToString(), out floatObj))
        {
            GameObject curPrefab=GameObject.Instantiate(floatObj,parent);
            //curPrefab.transform.SetParent(parent,true);
            curPrefab.GetComponent<FloatingScore>().Init(score);
            return curPrefab;
        }

        return null;
    }

    public GameObject InstantiateGridPrefab(string guideName,Transform parent)
    {
        GameObject gridObj;
        if (gridPrefabs.ContainsKey(guideName, out gridObj))
        {
            GameObject curPrefab = GameObject.Instantiate(gridObj, parent);
            //curPrefab
            return curPrefab;
        }
        return null;
    }
}
