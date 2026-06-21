using UnityEngine;
using UnityEngine.UI;
using SettingsElements;

public class SettingsMenu : MonoBehaviour
{
    [Header("BackButtonOnClick")]
    [SerializeField] private MenuManager menuManagerRef;

    [Header("AudioSettings")]
    [SerializeField] private Slider masterVolumeSliderIn, soundVolumeSliderIn, musicVolumeSliderIn;

    [Header("OtherSettings")]
    [SerializeField] private Slider mouseSensitivitySlider;
    private void OnEnable()
    {
        mouseSensitivitySlider.value = GlobalDataStore.GetSettingsData().mouseSensitivity;
        mouseSensitivitySlider.onValueChanged.AddListener((value) => GlobalDataStore.GetSettingsData().mouseSensitivity = value);
    }

    private void Start()
    {
        new AudioVolumeSlider(masterVolumeSliderIn, AudioUtil.Constants.masterVolumeParameter);
        new AudioVolumeSlider(soundVolumeSliderIn, AudioUtil.Constants.soundVolumeParameter);
        new AudioVolumeSlider(musicVolumeSliderIn, AudioUtil.Constants.musicVolumeParameter);
    }

    public void BackButtonOnClick()
    {
        GlobalDataStore.Instance.settingsManager.Save();
        gameObject.SetActive(false);
        if (GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen)
            menuManagerRef.titleMenuRef.SetActive(true);
        else
            menuManagerRef.pauseMenuRef.SetActive(true);
    }

    void Update()
    {
        if (GlobalDataStore.GetStateManager().playerState.player == null)
            return;

        if (GlobalDataStore.GetStateManager().playerState.player.PauseActionRef.InteractWithCooldown())
            BackButtonOnClick();

    }
}
