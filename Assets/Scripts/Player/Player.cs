using UnityEngine;
using Entity;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private float itemDropDistance = 1.5f;
    private AudioListener audioListener;
    private InputAction pauseAction;
    private void Start()
    {
        healthBar.SetOnDeathCallback(OnPlayerDeath);
        pauseAction = InputSystem.actions.FindAction("Pause");
        audioListener = gameObject.AddComponent<AudioListener>();
        EnableGamePlay();

        DebugDev.DebugFunction.RegisterDebugCallback(() => DropItem("testStoreInternal"));
    }
    private void Update()
    {
        if(GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal)
            if(GlobalDataStore.GetStateManager().player.unLoadPauseMenuSceneCount != SceneManager.sceneCount)
            {
                GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal = false;
                EnableGamePlay();
            }

        healthBar.Update();
        if (pauseAction.IsPressed())
        {
            LoadPauseMenu();
            return;
        }
    }
    public void LoadPauseMenu()
    {
        DisableGamePlay();
        audioListener.enabled = false;

        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget = playerRef.playerCamera;
        SceneManager.LoadScene(StateManager.MenuManger.MenuMangerScenceId,LoadSceneMode.Additive);
    }
    public void DisableGamePlay()
    {
        Time.timeScale = 0f;
        DisableInputActions();
    }
    public void EnableGamePlay()
    {
        EnableInputActions();
        audioListener.enabled = true;
        Time.timeScale = 1f;
    }
    public void DropItem(string internalName)
    {
        if(!GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName,out ItemData itemData))
            return;
        if(!itemData.storeable || itemData.storeableSpawnPrefab == null)
            return;
        if(!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName,out InventoryItem inventoryItem))
            return;
        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        if(!ItemEntity.TryDropItem(playerRef.playerCamera,itemData,itemEntityIdNewPrefab,itemDropDistance))
            return;

        GlobalDataStore.GetInventory().DropItem(internalName,out _);
    }
    public void DamagePlayer(float damageAmount) { healthBar.ReduceHealth(damageAmount); }
    public void HealPlayer(float healAmount) { healthBar.IncreaseHealth(healAmount); }

    public void DisableInputActions()
    {
        InputSystem.actions.FindActionMap("Player").Disable();
        Cursor.lockState = CursorLockMode.None;
    }
    public void EnableInputActions()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
        Cursor.lockState = CursorLockMode.Locked;
    
    }
    private void OnPlayerDeath()
    {
        return;
    }
}