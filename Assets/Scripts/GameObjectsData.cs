using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GamePrefabs/PrefabsDatas", order = 0)]
public class GameObjectsData : ScriptableObject
{
    //[SerializeField]
    //private List<string> prefabPaths = new List<string>();
    //[HideInInspector]
    //public Dictionary<string, string> prefabPathDictionary = new Dictionary<string, string>();
    //public void Initialize()
    //{
    //    if (prefabPathDictionary.Count > 0)
    //        return;
    //    prefabPathDictionary.Clear();
    //    foreach (string path in prefabPaths)
    //    {
    //        string[] parts = path.Split('/');
    //        string key = parts[parts.Length - 1].Replace(".prefab", ""); // 例如 "Grid/Blue" 变为 "Blue"
    //        prefabPathDictionary[key] = path;
    //    }
    //}
    //public GameObject GetPrefab(string key)
    //{
    //    if (prefabPathDictionary.TryGetValue(key, out string path))
    //    {
    //        return Resources.Load<GameObject>(path);
    //    }
    //    Debug.LogError($"Prefab for '{key}' not found.");
    //    return null;
    //}


    [SerializeField]
    public List<GameObject> objList;
    [HideInInspector]
    public Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    public void Initialize()
    {
        if (prefabDictionary.Count > 0)
            return;
        prefabDictionary.Clear();
        foreach (GameObject obj in objList)
        {
            prefabDictionary[obj.name] = obj;
        }
    }

    public bool ContainsKey(string key, out GameObject value)
    {
        return prefabDictionary.TryGetValue(key, out value);
    }

    public GameObject GetPrefab(string key)
    {
        return ContainsKey(key, out GameObject value) ? value : null;
    }
}

