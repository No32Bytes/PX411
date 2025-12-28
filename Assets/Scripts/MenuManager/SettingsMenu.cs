using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using SettingsElements;

public class SettingsMenu : MonoBehaviour
{
    [Header("BackButtonOnClick")]
    [SerializeField] private GameObject titleMenuReference;
    [SerializeField] private GameObject pauseMenuReference;

    [Header("AudioSettings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSliderIn, soundVolumeSliderIn, musicVolumeSliderIn;

    private void Start()
    {
        AudioVolumeSlider.SetGlobalAudioMixer(audioMixer);
        new AudioVolumeSlider(masterVolumeSliderIn,AudioUtil.Constants.masterVolumeParameter);
        new AudioVolumeSlider(soundVolumeSliderIn,AudioUtil.Constants.soundVolumeParameter);
        new AudioVolumeSlider(musicVolumeSliderIn, AudioUtil.Constants.musicVolumeParameter);
    }

    public void BackButtonOnClick()
    {
        GlobalDataStore.Instance.settingsManager.Save();
        gameObject.SetActive(false);
        if (GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen)
            titleMenuReference.SetActive(true);
        else 
            pauseMenuReference.SetActive(true);
    }
}
