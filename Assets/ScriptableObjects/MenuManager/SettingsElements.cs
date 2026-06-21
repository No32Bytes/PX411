using UnityEngine.Audio;
using UnityEngine.UI;

namespace SettingsElements
{
    public sealed class AudioVolumeSlider
    {
        private readonly Slider volumeSlider_;
        private readonly string volumeParameter_;
        public AudioVolumeSlider(Slider volumeSlider, string volumeParameter)
        {
            volumeSlider_ = volumeSlider;
            volumeParameter_ = volumeParameter;
            volumeSlider_.value = GetSettingsVolume();
            var audioMixer = GlobalDataStore.Instance.masterMixer;
            audioMixer.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(GetSettingsVolume()));

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

            var audioMixer = GlobalDataStore.Instance.masterMixer;
            audioMixer.SetFloat(volumeParameter_, AudioUtil.ConvertRawVolumeToVolume(volume));
            SetSettingsVolume(volume);
        }
    };

}