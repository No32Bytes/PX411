using Entity;
using InputUtil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float toggleHandyUIActionCooldownS;
    private InputHandlerCooldown toggleHandyUIAction;
    private InputHandler pauseAction;
    private AudioListener audioListener;

    private void Awake()
    {
        audioListener = gameObject.AddComponent<AudioListener>();
        healthBar.SetOnDeathCallback(OnPlayerDeath);
    }

    private void Start()
    {
        pauseAction = new("Pause");
        toggleHandyUIAction = new("ToggleHandyUI", toggleHandyUIActionCooldownS,InputHandlerCooldown.CooldownType.TimeUnscaled);

        GlobalDataStore.GetStateManager().playerState.player = this;
        EnableGamePlay();
    }

    private void Update()
    {
        healthBar.Update();

        if (GlobalDataStore.GetStateManager().playerState.signalUnloadPauseMenu.Valid())
        {
            GlobalDataStore.GetStateManager().playerState.signalUnloadPauseMenu.Reset();
            EnableGamePlay();
        }

        if (toggleHandyUIAction.InteractWithCooldown())
        {
            if(audioListener.enabled == false)
                return;
                
            if(Time.timeScale == 0)
            {
                DisableHandyScreenUI();
                return;
            }
            EnableHandyScreenUI();
        }
            
        

        if (pauseAction.IsPressed())
            LoadPauseMenu();
            
        
    }

    private void LoadPauseMenu()
    {
        DisableGamePlay();
        audioListener.enabled = false;

        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget = playerRef.playerCamera;
        SceneManager.LoadScene(GlobalDataStore.GetStateManager().menuManger.MenuMangerScenceId, LoadSceneMode.Additive);
    }
    private void DisableGamePlay()
    {
        Time.timeScale = 0;
        InputSystem.actions.FindActionMap("Player").Disable();
        Cursor.lockState = CursorLockMode.None;
    }
    private void EnableGamePlay()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
        Cursor.lockState = CursorLockMode.Locked;
        audioListener.enabled = true;
        Time.timeScale = 1f;
    }
    public void EnableHandyScreenUI()
    {
        if(Time.timeScale == 0f) 
            return;
        
        DisableGamePlay();
        playerRef.handyScreenUI.SetActive(true);
    }
    public void DisableHandyScreenUI()
    {
        playerRef.handyScreenUI.SetActive(false);
        EnableGamePlay();
    }

    private void OnPlayerDeath()
    {

    }

    public void Damage(float damageAmount)
    {
        healthBar.ReduceHealth(damageAmount);
    }

    public void Heal(float healAmount)
    {
        healthBar.IncreaseHealth(healAmount);
    }

}