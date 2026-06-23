using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MenuManager : MonoBehaviour
{

    public GameObject titleMenuRef;
    public GameObject pauseMenuRef;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private SoundTrack mainMenuThemeGroup;

    private void Awake()
    {
        bool titleMenuOpen = GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen;
        mainCamera.enabled = titleMenuOpen;
        if (GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget)
            GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget.enabled = !titleMenuOpen;
        titleMenuRef.SetActive(titleMenuOpen);
        pauseMenuRef.SetActive(!titleMenuOpen);

        if (GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen)
        {
            return;
        }

        AddOverlayCameraToTargetCamera();
    }
    public void Start()
    {
        GlobalDataStore.GetAudioManager().PlaySoundTrackGroup(mainMenuThemeGroup);
        GlobalDataStore.Instance.settingsManager.LoadVolumeSettings();

        if (GlobalDataStore.GetStateManager().menuManger.loadIntoGameScenceDirect)
        {
            GlobalDataStore.GetStateManager().menuManger.loadIntoGameScenceDirect = false;
            titleMenuRef.GetComponent<TitleMenu>().StartButtonOnClick();
            return;
        }
    }
    public void ForceAwake()
    {
        Awake();
    }
    public void AddOverlayCameraToTargetCamera()
    {
        Camera targetCamera = GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget;
        targetCamera.GetUniversalAdditionalCameraData().cameraStack.Add(menuCamera);
    }
    public void RemoveOverlayCameraFromTargetCamera()
    {
        Camera targetCamera = GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget;
        targetCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(menuCamera);
    }
}
