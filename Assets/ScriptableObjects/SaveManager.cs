using System.Collections.Generic;
using System.IO;

public class SaveData
{
    public int a = 1;
};
public class SaveManager
{
    private static readonly string defaultSaveID = "default";
    private readonly string persistentDataPath;
    public string CurrentSaveID { get; private set; }
    public SaveData currentSave;
    public SaveManager(string persistentDataPath)
    {
        this.persistentDataPath = persistentDataPath;
        Load(GlobalDataStore.GetSettingsData().lastSaveID);
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
        SaveUtil.SaveObjectToFile(GetSaveDataPath(CurrentSaveID), currentSave);
    }
    public void Load(string saveID,bool saveBeforeLoad = true)
    {
        if (CurrentSaveID == saveID) return;
        if (!string.IsNullOrEmpty(CurrentSaveID) && saveBeforeLoad == true)
            Save();

        CurrentSaveID = saveID;
        if (SaveUtil.LoadObjectFromFile(GetSaveDataPath(saveID), out currentSave))
            currentSave ??= new();
        else
            currentSave = new();

        GlobalDataStore.GetSettingsData().lastSaveID = saveID;
    }
    public void Delete(string saveID)
    {
        if (CurrentSaveID == defaultSaveID && CurrentSaveID == saveID)
        {
            currentSave = new();
            return;
        }

        SaveUtil.DeleteFile(GetSaveDataPath(saveID));
        if (CurrentSaveID != saveID) return;

        string loadSaveID = defaultSaveID;
        if(GetExistingSaveIDs().Count > 0)
            loadSaveID = GetExistingSaveIDs()[0];
        Load(loadSaveID,false);
    }
    public bool PeekSaveData(string saveID, out SaveData saveData)
    {
        return SaveUtil.LoadObjectFromFile(GetSaveDataPath(saveID), out saveData);
    }
};