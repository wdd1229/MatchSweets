using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ×©¿é¹ÜÀí
/// </summary>
public class WallManager : MonoBehaviour
{
    private GameObject walls;
    private GameObject prefab;
    private void Awake()
    {
        prefab = Resources.Load<GameObject>("wall");
    }

    public void CreatWallOfLevelData(int wallCount)
    {
        for (int i = 0; i < wallCount; i++)
        {
            GameObject.Instantiate(prefab,transform);
        }
    }
}
