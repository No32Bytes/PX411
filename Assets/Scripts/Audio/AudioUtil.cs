using UnityEngine;
public class AudioUtil
{
    public static float ConvertRawVolumeToVolume(float infloat)
    {
        return Mathf.Log10(infloat) * 20;
    }
    public static float ConvertVolumeToRawVolume(float volume)
    {
        return Mathf.Pow(10, volume / 20);
    }
    public struct Constants
    {
        public const string masterVolumeParameter = "MasterVolume";
        public const string soundVolumeParameter = "SoundVolume";
        public const string musicVolumeParameter = "MusicVolume";

        public const string masterMixerGroup = "Master";
        public const string musicMixerGroup = "Music";
        public const string soundMixerGroup = "Sound";

    }

}

