// 推荐做法：使用ScriptableObject资源清单
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/PrefabManifest")]
public class PrefabManifest : ScriptableObject
{
    public GameObject[] prefabs;

    public GameObject GetPrefab(string name)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.name == name) return prefab;
        }
        return null;
    }
}
