using UnityEngine.Audio;
using UnityEngine.UI;

namespace SettingsMenuHelper
{
    public sealed class  AudioVolumeSlider
    {
        private static AudioMixer audioMixer_;
        private readonly Slider volumeSlider_;
        private readonly string volumeParameter_;
        public static void SetGlobalAudioMixer(AudioMixer audioMixer)
        {
            audioMixer_ = audioMixer;
        }
        public AudioVolumeSlider(Slider volumeSlider, string volumeParameter)
        {
            volumeSlider_ = volumeSlider;
            volumeParameter_ = volumeParameter;
            volumeSlider_.value = GetSettingsVolume();
            audioMixer_.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(GetSettingsVolume()));

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
            audioMixer_.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(volume));
            SetSettingsVolume(volume);
        }
    };

}