using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadJson<T> : MonoBehaviour
{
    /// <summary>
    /// 从Json文件中读取数据
    /// </summary>
    /// <param name="_FileName">文件名（文件必须是.json文件）</param>
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
