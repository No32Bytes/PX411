using System.IO;
using UnityEngine.InputSystem;

public class SettingsData
{
    public string lastSaveID = "default";
    public string bindingOverridesJson = "";
    public float mouseSensitivity = 10f;
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
        settingsData.bindingOverridesJson = InputSystem.actions.SaveBindingOverridesAsJson();
        SaveUtil.SaveObjectToFile(GetSettingsDataPath(), settingsData);
    }
    public void Reset()
    {
        settingsData = new();
    }
    public void Load()
    {
        SaveUtil.LoadObjectFromFile(GetSettingsDataPath(), out settingsData);

        if (settingsData != null)
            if (settingsData.bindingOverridesJson != "")
                InputSystem.actions.LoadBindingOverridesFromJson(settingsData.bindingOverridesJson);

        settingsData ??= new();

    }
    public void LoadVolumeSettings()
    {
        var audioMixer = GlobalDataStore.Instance.masterMixer;
        audioMixer.SetFloat(AudioUtil.Constants.masterVolumeParameter, AudioUtil.ConvertRawVolumeToVolume(settingsData.audioMasterVolume));
        audioMixer.SetFloat(AudioUtil.Constants.musicVolumeParameter, AudioUtil.ConvertRawVolumeToVolume(settingsData.audioMusicVolume));
        audioMixer.SetFloat(AudioUtil.Constants.soundVolumeParameter, AudioUtil.ConvertRawVolumeToVolume(settingsData.audioSoundVolume));
    }
}