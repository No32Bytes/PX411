using System;
using UnityEngine;
public class AudioUtil
{
    [Serializable]
    public class RandomRange
    {
        [SerializeField] private float min;
        [SerializeField] private float max;
        public float GetRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
    public static float ConvertRawVolumeToVolume(float infloat)
    {
        return Mathf.Log10(infloat) * 20;
    }
    public static float ConvertVolumeToRawVolume(float volume)
    {
        return Mathf.Pow(10, volume / 20);
    }
    public static ref float GetSettingsVolumeRef(string volumeParameter)
    {
        if (volumeParameter == Constants.masterVolumeParameter)
            return ref GlobalDataStore.GetSettingsData().audioMasterVolume;
        if (volumeParameter == Constants.musicVolumeParameter)
            return ref GlobalDataStore.GetSettingsData().audioMusicVolume;
        return ref GlobalDataStore.GetSettingsData().audioSoundVolume;
    }
    public struct Constants
    {
        public const float defaultVolume = 0.5f;
        public const string masterVolumeParameter = "MasterVolume";
        public const string soundVolumeParameter = "SoundVolume";
        public const string musicVolumeParameter = "MusicVolume";

        public const string masterMixerGroup = "Master";
        public const string musicMixerGroup = "Music";
        public const string soundMixerGroup = "Sound";
    }

}

