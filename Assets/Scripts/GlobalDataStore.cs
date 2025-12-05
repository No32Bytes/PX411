using UnityEngine;
public class GlobalDataStore : MonoBehaviour
{
    public SaveManager saveManager;
    public MenuManager menuManager = new();
    public static GlobalDataStore Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        saveManager = new(Application.persistentDataPath);
        DontDestroyOnLoad(gameObject);
    }
    public class MenuManager 
    {
        public bool TitleMenuOpen = true;
    };
}