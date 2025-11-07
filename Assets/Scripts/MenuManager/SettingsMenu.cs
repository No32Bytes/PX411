using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    internal class AudioVolumeSlider
    {
        public static AudioMixer AudioMixer_ { get; set; }
        private readonly Slider volumeSlider_;
        public readonly string volumeParameter_;
        public AudioVolumeSlider(Slider volumeSlider, string volumeParameter)
        {
            volumeSlider_ = volumeSlider;
            volumeParameter_ = volumeParameter;
            if (AudioMixer_.GetFloat(volumeParameter_, out float currentVolume))
                volumeSlider_.value = AudioUtil.ConvertVolumeToRawVolume(currentVolume);

            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        private void OnSliderChanged(float volume)
        {
            if (volume == 0)
                volume = -180;
            AudioMixer_.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(volume));
            
        }
    };
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


}
