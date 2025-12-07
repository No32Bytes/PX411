using System;
using System.Collections.Generic;
using System.IO;

public class SaveData
{
    public int a = 1;
};
public class SaveManager
{
    private readonly string persistentDataPath;
    private string currentSaveID;
    public SaveData currentSave;
    public SaveManager(string persistentDataPath)
    {
        this.persistentDataPath = persistentDataPath;
        Load(GlobalDataStore.Instance.settingsManager.settingsData.lastSaveID);
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
    public void Load(string saveID)
    {
        if (currentSaveID == saveID) return;
        if (!string.IsNullOrEmpty(currentSaveID))
            Save();

        currentSaveID = saveID;
        if (!File.Exists(GetSaveDataPath(saveID)))
        {
            currentSave = new();
            Save();
        }
        else
        {
            currentSave = SaveUtil.LoadObjectFromFile<SaveData>(GetSaveDataPath(saveID));
            if (currentSave == null)
            {
                currentSave = new();
                Save();
            }
        }
        GlobalDataStore.Instance.settingsManager.settingsData.lastSaveID = saveID;
    }
    public bool PeekSaveData(string saveID, out SaveData saveData)
    {
        saveData = SaveUtil.LoadObjectFromFile<SaveData>(GetSaveDataPath(saveID));
        return saveData != null;
    }
};