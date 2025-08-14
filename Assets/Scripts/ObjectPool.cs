using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GridType
{
    Blue,
    Yellow,
    Green,
    Red,
    Orange,
    Empty,
    Null//����Null�ɴ����ո���Ҳ���Ǳ�����
}
/// <summary>
/// �����
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;

    public Dictionary<GridType,List<GameObject>> gridPrefabDict;

    //��ʹ��ʱ�ж��Ƿ�Ϊ�� ���򴴽�Ԥ�ƣ����ڽű�
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(ObjectPool).Name);
                instance = obj.AddComponent<ObjectPool>();
                DontDestroyOnLoad(obj); // ��֤�糡������
            }
            return instance;
        }
    }

    private void Awake()
    {
        gridPrefabDict=new Dictionary<GridType,List<GameObject>>();
    }

    /// <summary>
    /// �Ӷ�����л�ȡ����ʹ��
    /// </summary>
    /// <param name="GridType"></param>
    /// <returns></returns>
    public GameObject GetObject(GridType GridType)
    {
        GameObject obj;
        if (gridPrefabDict.ContainsKey(GridType))
        {
            int idx = gridPrefabDict[GridType].Count-1;
            obj=gridPrefabDict[GridType][idx];
            gridPrefabDict[GridType].Remove(obj);
        }
        else
        {
            obj=null;
            Debug.Log($"��ǰ����:{GridType} û�б���ǰ������ͣ������ʼ��");
        }
        return obj;
    }

    /// <summary>
    /// ����Դ�ŵ��������
    /// </summary>
    /// <param name="GridType"></param>
    /// <param name="obj"></param>
    public void PutObject(GridType GridType, GameObject obj) 
    {
        if (gridPrefabDict.ContainsKey(GridType))
        {
            gridPrefabDict[GridType].Add(obj);
        }
    }
}
