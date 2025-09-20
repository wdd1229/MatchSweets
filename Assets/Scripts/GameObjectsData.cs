using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameObject合集", order = 0)]
public class GameObjectsData : ScriptableObject
{

    [SerializeField]
    public List<GameObject> objList;

    public bool ContainsKey(string key,out GameObject prefab)
    {
        foreach (GameObject obj in objList)
        {
            if(obj.name == key)
            {
                prefab = obj;
                return true;
            }
        }
        prefab = null;
        return false;
    }
}
