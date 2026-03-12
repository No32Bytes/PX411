using UnityEngine;
using Entity;
using InputUtil;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private float itemDropDistance = 1.5f;
    [SerializeField] private float toggleHandyUIActionCooldownS;
    [SerializeField] private float toggleHandyActionCooldownS;
    [SerializeField] private AnimationParamterInfo toggleHandyAnimationParamter;
    private InputHandlerCooldown toggleHandyAction, toggleHandyUIAction;
    private AudioListener audioListener;
    private InputHandler pauseAction;
    private void Awake()
    {
        audioListener = gameObject.AddComponent<AudioListener>();
        healthBar.SetOnDeathCallback(OnPlayerDeath);
    }
    private void Start()
    {
        pauseAction = new("Pause");
        toggleHandyAction = new("ToggleHandy", toggleHandyActionCooldownS);
        toggleHandyUIAction = new("ToggleHandyUI", toggleHandyUIActionCooldownS);

        playerRef.handyScreenUI.SetActive(false);
        GlobalDataStore.GetStateManager().player.playerReference = this;

        EnableGamePlay();
    }
    private void Update()
    {
        healthBar.Update();

        if (GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal)
            HandleUnLoadPauseMenuSignal();

        if (toggleHandyUIAction.InteractWithCooldown())
            HandyUISetActive(true);



        if (toggleHandyAction.InteractWithCooldown())
            toggleHandyAnimationParamter.ValueBool = !toggleHandyAnimationParamter.ValueBool;

        if (pauseAction.IsPressed())
            LoadPauseMenu();

    }
    private void HandleUnLoadPauseMenuSignal()
    {
        if (GlobalDataStore.GetStateManager().player.unLoadPauseMenuSceneCount != SceneManager.sceneCount)
        {
            GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal = false;
            EnableGamePlay();
        }
    }

    private void LoadPauseMenu()
    {
        DisableGamePlay();
        audioListener.enabled = false;

        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget = playerRef.playerCamera;
        SceneManager.LoadScene(GlobalDataStore.GetStateManager().menuManger.MenuMangerScenceId, LoadSceneMode.Additive);
    }
    public bool DropItem(string internalName)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        if (!itemData.storeable || itemData.storeableSpawnPrefab == null)
            return false;
        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        return ItemEntity.TryDropItem(playerRef.playerCamera, itemData, itemEntityIdNewPrefab, itemDropDistance);
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
    public void HandyUISetActive(bool active)
    {
        if (SceneManager.sceneCount != 1)
            return;

        if (active)
        {
            DisableGamePlay();
            playerRef.handyScreenUI.SetActive(true);
            return;
        }
        playerRef.handyScreenUI.SetActive(false);
        EnableGamePlay();
    }
}