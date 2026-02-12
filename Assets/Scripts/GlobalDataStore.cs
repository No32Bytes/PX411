using UnityEngine;
[DefaultExecutionOrder(-1)]
public class GlobalDataStore : MonoBehaviour
{
    public ItemDataBase itemDataBase;
    public SaveManager saveManager;
    public SettingsManager settingsManager;
    public StateManager stateManager;
    public static GlobalDataStore Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Initalize();
    }
    private void OnApplicationQuit()
    {
        settingsManager.Save();
        saveManager.Save();
    }
    private void Initalize()
    {
        settingsManager = new(Application.persistentDataPath);
        saveManager = new(Application.persistentDataPath);
        stateManager = new();
    }
    public static ItemDataBase GetItemDataBase()
    {
        return Instance.itemDataBase;
    }
    public static SettingsData GetSettingsData()
    {
        return Instance.settingsManager.settingsData;
    }
    public static SaveData GetSaveData()
    {
        return Instance.saveManager.currentSave;
    }
    public static StateManager GetStateManager()
    {
        return Instance.stateManager;
    }
}