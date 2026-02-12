using System;
using System.IO;
using UnityEngine;
public class SaveUtil
{
    public static bool SaveObjectToFile(string savePath, object obj)
    {
        string json = JsonUtility.ToJson(obj);
        try
        {
            File.WriteAllText(savePath, json, System.Text.Encoding.ASCII);
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to Write to path {savePath} because of error {e.Message}");
            return false;
        }
        Debug.Log($"Successfully written to {savePath}");
        return true;
    }
    public static bool LoadObjectFromFile<T>(string savePath, out T data)
    {
        data = default;
        if (!File.Exists(savePath)) return false;

        string json;
        try
        {
            json = File.ReadAllText(savePath, System.Text.Encoding.ASCII);
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to Load File with Error: {e.Message}");
            return false;
        }

        data = JsonUtility.FromJson<T>(json);
        Debug.Log($"Successfully loaded data from {savePath}");
        return true;
    }
    public static void DeleteFile(string path)
    {
        if (!File.Exists(path)) return;
        File.Delete(path);
    }
};