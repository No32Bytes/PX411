using System;
using UnityEngine;
public class AudioUtil
{
    [Serializable]
    public class RandomRange
    {
        [SerializeField] private float min = 1f;
        [SerializeField] private float max = 1f;
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
    public static void PlaySoundEffect(BaseSoundEffect soundEffect, AudioSource source)
    {
        if (soundEffect == null)
            return;
        if (source == null)
            return;
        if (source.isPlaying)
            return;
        soundEffect.Play(source);
    }
    public static AudioSource CreateSoundEffectAudioSource(GameObject gameObject, float maxDistance = 30f)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1.0f;
        source.maxDistance = maxDistance;
        source.clip = null;
        var mixerGroup = GlobalDataStore.GetAudioManager().SoundMixerGroup;
        if (mixerGroup != null)
            source.outputAudioMixerGroup = mixerGroup;
        return source;
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

