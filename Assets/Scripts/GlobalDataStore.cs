using UnityEngine;
[DefaultExecutionOrder(-1)]

[RequireComponent(typeof(AudioManager))]
public class GlobalDataStore : MonoBehaviour
{
    public ItemDataBase itemDataBase;
    public GameObject entityInformationViewInteractInfo;
    public Behaviour[] awakeComponents;
    public SaveManager saveManager;
    public SettingsManager settingsManager;
    public StateManager stateManager;
    public static GlobalDataStore Instance { get; private set; }

    private AudioManager audioManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Behaviour component in awakeComponents)
            component.enabled = true;
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
        audioManager = GetComponent<AudioManager>();
        if (!gameObject.TryGetComponent(out stateManager.eventSystem))
            throw new System.Exception("GlobalDataStore must be possess an Eventsystem must be manually added");

        DebugDev.DebugFunction.Start();
    }
    private void Update()
    {
        DebugDev.DebugFunction.Update();
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
    public static Inventory GetInventory()
    {
        return Instance.saveManager.currentSave.inventory;
    }
    public static AudioManager GetAudioManager()
    {
        return Instance.audioManager;
    }

}