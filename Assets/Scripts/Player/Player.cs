using Entity;
using InputUtil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float togglePauseMenuCooldownS = 0.5f;
    [SerializeField] private float toggleHandyUIActionCooldownS = 0.1f;
    [SerializeField] float damageDelay;
    [Header("Sound")]
    [SerializeField] private BaseSoundEffect damageSound;
    [SerializeField] private BaseSoundEffect deathSound;
    private InputHandlerCooldown toggleHandyUIAction;
    public InputHandlerCooldown pauseAction;
    private AudioListener audioListener;
    private AudioSource audioSource;
    public AudioSource OverrideDamageAudioSource => audioSource;
    public InputHandlerCooldown PauseActionRef => pauseAction;
    private float lastDamage;
    private bool isDead;

    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        audioListener = gameObject.AddComponent<AudioListener>();
        healthBar.SetOnDeathCallback(OnPlayerDeath);

        GlobalDataStore.GetStateManager().playerState.player = this;
        isDead = false;
    }

    private void Start()
    {
        pauseAction = new("Pause", togglePauseMenuCooldownS, InputHandlerCooldown.CooldownType.TimeUnscaled);
        toggleHandyUIAction = new("ToggleHandyUI", toggleHandyUIActionCooldownS, InputHandlerCooldown.CooldownType.TimeUnscaled);

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
            if (audioListener.enabled == false)
                return;

            if (Time.timeScale == 0)
            {
                DisableHandyScreenUI();
                return;
            }
            EnableHandyScreenUI();
        }


        if (audioListener.enabled == true)
            if (pauseAction.InteractWithCooldown())
            {
                DisableHandyScreenUI();
                LoadPauseMenu();
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
        if (Time.timeScale == 0f)
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
        if (isDead)
            return;

        audioSource.Stop();
        AudioUtil.PlaySoundEffect(deathSound, audioSource);
        isDead = true;
    }

    public void Damage(float damageAmount)
    {
        if (isDead)
            return;
        if (lastDamage + damageDelay > Time.time)
            return;

        lastDamage = Time.time;
        AudioUtil.PlaySoundEffect(damageSound, audioSource);
        healthBar.ReduceHealth(damageAmount);

    }

    public void Heal(float healAmount)
    {
        healthBar.IncreaseHealth(healAmount);
    }

}