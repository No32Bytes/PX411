using System.IO;
using UnityEngine;
public class SaveUtil
{
    public static void SaveObjectToFile(string savePath, object obj)
    {
        string json = JsonUtility.ToJson(obj);
        File.WriteAllText(savePath, json,System.Text.Encoding.ASCII);
        Debug.Log("Writing to path " + savePath + " Data: " + json);
    }
    public static T LoadObjectFromFile<T>(string savePath)
    {
        string json = File.ReadAllText(savePath,System.Text.Encoding.ASCII);
        return JsonUtility.FromJson<T>(json);
    }
    public static void DeleteFile(string path)
    {
        File.Delete(path);
    }
};