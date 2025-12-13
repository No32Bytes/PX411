using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    internal class AudioVolumeSlider
    {
        public static AudioMixer AudioMixer_ { get; set; }
        private readonly Slider volumeSlider_;
        private readonly string volumeParameter_;
        public AudioVolumeSlider(Slider volumeSlider, string volumeParameter)
        {
            volumeSlider_ = volumeSlider;
            volumeParameter_ = volumeParameter;
            volumeSlider_.value = GetSettingsVolume();
            AudioMixer_.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(GetSettingsVolume()));

            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        private float GetSettingsVolume()
        {
            return AudioUtil.GetSettingsVolumeRef(volumeParameter_);
        }
        private void SetSettingsVolume(float volume)
        {
            AudioUtil.GetSettingsVolumeRef(volumeParameter_) = volume;
        }
        private void OnSliderChanged(float volume)
        {
            if (volume == 0)
                volume = -180;
            AudioMixer_.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(volume));
            SetSettingsVolume(volume);
        }
    };
    [Header("MenuMangerReferences")]
    [SerializeField] private GameObject titleMenuReference;
    [SerializeField] private GameObject pauseMenuReference;

    [Header("AudioReferences")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSliderIn, soundVolumeSliderIn, musicVolumeSliderIn;
    private AudioVolumeSlider masterVolumeSlider, soundVolumeSlider, musicVolumeSlider;

    private void Start()
    {
        AudioVolumeSlider.AudioMixer_ = audioMixer;
        masterVolumeSlider = new(masterVolumeSliderIn,AudioUtil.Constants.masterVolumeParameter);
        soundVolumeSlider = new(soundVolumeSliderIn,AudioUtil.Constants.soundVolumeParameter);
        musicVolumeSlider = new(musicVolumeSliderIn, AudioUtil.Constants.musicVolumeParameter);
    }

    public void BackButtonOnClick()
    {
        GlobalDataStore.Instance.settingsManager.Save();
        gameObject.SetActive(false);
        if (GlobalDataStore.Instance.menuManager.TitleMenuOpen)
            titleMenuReference.SetActive(true);
        else 
            pauseMenuReference.SetActive(true);
    }
}
