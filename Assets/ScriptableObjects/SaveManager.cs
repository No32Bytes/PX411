using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData
{
    public int a = 1;
};
public class SaveManager
{
    private readonly string persistentDataPath;
    private string currentSaveID;
    private SaveData currentSave;
    public SaveManager(string persistentDataPath)
    {
        this.persistentDataPath = persistentDataPath;
        Load("default");
    }
    private string GetSaveDataPath(string saveID)
    {
        return persistentDataPath + Path.DirectorySeparatorChar + "save" + saveID + ".dat";
    }
    public List<string> GetExistingSaveIDs()
    {
        List<string> existingSaveIDs = new();
        foreach (string saveFileName in Directory.EnumerateFiles(persistentDataPath, "save*.dat"))
            existingSaveIDs.Add(Path.GetFileNameWithoutExtension(saveFileName)[4..]);

        return existingSaveIDs;
    }
    public void Save()
    {
        SaveUtil.SaveObjectToFile(GetSaveDataPath(currentSaveID), currentSave);
    }
    public bool Load(string saveID)
    {
        if (currentSaveID == saveID) return true;
        Save();
        currentSaveID = saveID;
        if (!File.Exists(GetSaveDataPath(saveID)))
        {
            currentSave = new();
            return true;
        }
        currentSave = SaveUtil.LoadObjectFromFile<SaveData>(GetSaveDataPath(saveID));
        currentSave ??= new();
        return true;
    }
};