using UnityEngine;
using Entity;
using InputUtil;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private float itemDropDistance = 1.5f;
    private AudioListener audioListener;
    private InputHandler pauseAction,viewHandyAction;
    private void Start()
    {
        GlobalDataStore.GetStateManager().player.playerReference = this;

        pauseAction = new("Pause");
        viewHandyAction = new("ViewHandy");

        healthBar.SetOnDeathCallback(OnPlayerDeath);
        audioListener = gameObject.AddComponent<AudioListener>();
        EnableGamePlay();
    }
    private void Update()
    {
        healthBar.Update();
        
        if(GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal)
            if(GlobalDataStore.GetStateManager().player.unLoadPauseMenuSceneCount != SceneManager.sceneCount)
            {
                GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal = false;
                EnableGamePlay();
            }

        if (pauseAction.IsPressed())
        {
            LoadPauseMenu();
            return;
        }
        if(viewHandyAction.IsPressed())
        {
            ViewHandyLarge();
            return;
        }
    }

    private void ViewHandyLarge()
    {
        DisableGamePlay();
        playerRef.playerCamera.GetUniversalAdditionalCameraData().cameraStack.Add(playerRef.handyScreenCamera);
    }
    public void RemoveHandyLarge()
    {
        playerRef.playerCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(playerRef.handyScreenCamera);
        EnableGamePlay();
    }

    private void LoadPauseMenu()
    {
        DisableGamePlay();
        audioListener.enabled = false;

        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget = playerRef.playerCamera;
        SceneManager.LoadScene(GlobalDataStore.GetStateManager().menuManger.MenuMangerScenceId,LoadSceneMode.Additive);
    }
    public bool DropItem(string internalName)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        if(!itemData.storeable || itemData.storeableSpawnPrefab == null)
            return false;
        if(!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName,out InventoryItem inventoryItem))
            return false;
        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        if(!ItemEntity.TryDropItem(playerRef.playerCamera,itemData,itemEntityIdNewPrefab,itemDropDistance))
            return false;

        GlobalDataStore.GetInventory().DropItem(internalName,out _);
        return true;
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
    public void DamagePlayer(float damageAmount) { healthBar.ReduceHealth(damageAmount); }
    public void HealPlayer(float healAmount) { healthBar.IncreaseHealth(healAmount); }
}