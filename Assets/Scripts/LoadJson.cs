using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadJson<T> : MonoBehaviour
{
    /// <summary>
    /// ��Json�ļ��ж�ȡ����
    /// </summary>
    /// <param name="_FileName">�ļ������ļ�������.json�ļ���</param>
    /// <returns></returns>
    public static T LoadJsonFromFile(string _FileName)
    {
        if (!File.Exists(Application.dataPath + "/Resources/Data/" + _FileName + ".json"))
        {
            return default(T);
        }

        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Data/" + _FileName + ".json");

        if (sr == null)
        {
            return default(T);
        }
        string json = sr.ReadToEnd();

        if (json.Length > 0)
        {
            return JsonUtility.FromJson<T>(json);
        }

        return default(T);
    }
}
