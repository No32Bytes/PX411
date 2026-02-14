using UnityEngine;
using Entity;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar = new();
    private AudioListener audioListener;
    private InputAction pauseAction;
    private void Start()
    {
        healthBar.SetOnDeathCallback(OnPlayerDeath);
        pauseAction = InputSystem.actions.FindAction("Pause");
        audioListener = gameObject.AddComponent<AudioListener>();
        EnableGamePlay();
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
        Time.timeScale = 0f;
        audioListener.enabled = false;
        DisableInputActions();
        
        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget = playerRef.playerCamera;
        SceneManager.LoadScene(StateManager.MenuManger.MenuMangerScenceId,LoadSceneMode.Additive);
    }
    public void EnableGamePlay()
    {
        EnableInputActions();
        audioListener.enabled = true;
        Time.timeScale = 1f;
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