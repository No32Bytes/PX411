using UnityEngine;
using System.IO;

public class SettingsData
{
    public string lastSaveID = "default";
    public float mouseSensitivity = 100f;
    public float audioMasterVolume = AudioUtil.Constants.defaultVolume;
    public float audioSoundVolume = AudioUtil.Constants.defaultVolume;
    public float audioMusicVolume = AudioUtil.Constants.defaultVolume;
};

public class SettingsManager
{
    private readonly string persistentDataPath;
    public SettingsData settingsData;
    public SettingsManager(string persistentDataPath)
    {
        this.persistentDataPath = persistentDataPath;
        Load();
    }
    private string GetSettingsDataPath()
    {
        return persistentDataPath + Path.DirectorySeparatorChar + "settings.dat";
    }
    public void Save()
    {
        SaveUtil.SaveObjectToFile(GetSettingsDataPath(), settingsData);
    }
    public void Reset()
    {
        settingsData = new();
    }
    public void Load()
    {
        if (!File.Exists(GetSettingsDataPath()))
        {
            settingsData = new();
            Save();
            return;
        }
        settingsData = SaveUtil.LoadObjectFromFile<SettingsData>(GetSettingsDataPath());
        settingsData ??= new();
    }
}