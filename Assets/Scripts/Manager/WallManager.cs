using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 砖块管理
/// </summary>
public class WallManager : MonoBehaviour
{
    private GameObject walls;
    private GameObject prefab;

    public GameObject cancelRoot;
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

    public void DesTroyWall()
    {
        if (transform.childCount > 0) {
            transform.GetChild(transform.childCount - 1).SetParent(cancelRoot.transform);
            Destroy(cancelRoot.transform.GetChild(cancelRoot.transform.childCount - 1).gameObject);
        }
    }

    public void DestroyAllWall()
    {
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        }

        for (int i = cancelRoot.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cancelRoot.transform.GetChild(cancelRoot.transform.childCount - 1).gameObject);
        }
    }
}
