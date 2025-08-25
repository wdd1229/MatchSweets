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

    /// <summary>
    /// 将对象写入JSON文件（与LoadJsonFromFile对应）
    /// </summary>
    /// <param name="_FileName">文件名（不含扩展名）</param>
    /// <param name="data">要写入的数据对象</param>
    public static void SaveJsonToFile(string _FileName, object data)
    {
        // 构建完整路径
        string directoryPath = Application.dataPath + "/Resources/Data/";
        string filePath = directoryPath + _FileName + ".json";

        try
        {
            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 序列化对象为JSON字符串
            string json = JsonUtility.ToJson(data, true); // 第二个参数为是否格式化（美化输出）

            // 写入文件
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(json);
            }

            Debug.Log($"JSON文件保存成功: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"保存JSON文件失败: {filePath}\n错误信息: {ex.Message}");
        }
    }
}
