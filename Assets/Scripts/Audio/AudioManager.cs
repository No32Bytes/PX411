using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    internal class MusicSoundTrack
    {
        public string soundTrackName;
        public string soundTrackGroup;
        public AudioClip audioClip;
        public float volume = 1.0f;
    };
    [SerializeField] private AudioMixer audioMixer;
    [Header("MusicManager")]
    [SerializeField] private bool playDefaultGroupAtEmpty;
    [SerializeField] private string defaultGroupName;
    [SerializeField] private MusicSoundTrack[] musicSoundTrackStore;
    public string CurrentDefaultGroupName { set; get; }
    private static readonly List<MusicSoundTrack> musicManagerQueue = new();
    public static AudioManager Instance { get; private set; }
    private static AudioSource musicAudioSource;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentDefaultGroupName = defaultGroupName;
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(AudioUtil.Constants.musicMixerGroup)[0];
    }

    private void FixedUpdate()
    {
        if (musicAudioSource.isPlaying)
            return;

        if (musicManagerQueue.Count == 0)
        {
            if (!HandleEmptyMusicQueue())
                return;
        }

        PlaySoundTrackFromQueue();
    }
    private bool HandleEmptyMusicQueue()
    {
        if (!playDefaultGroupAtEmpty)
            return false;
        musicManagerQueue.AddRange(Array.FindAll(musicSoundTrackStore, m => m.soundTrackGroup == CurrentDefaultGroupName));
        return musicManagerQueue.Count != 0;
    }
    public void PlaySoundTrackName(string soundTrackName)
    {
        int index = Array.FindIndex(musicSoundTrackStore, track => track.soundTrackName == soundTrackName);
        if (index == -1)
        {
            Debug.Log("soundTrackName could not be found");
            return;
        }
        PlaySoundTrack(musicSoundTrackStore[index]);
    }
    public void PlaySoundTrackGroup(string soundTrackGroup)
    {
        MusicSoundTrack[] soundTracks = Array.FindAll(musicSoundTrackStore, track => track.soundTrackGroup == soundTrackGroup);
        if (soundTracks.Length == 0)
        {
            Debug.Log("soundTrackGroup could not be found");
            return;
        }
        defaultGroupName = soundTrackGroup;
        musicManagerQueue.Clear();
        musicManagerQueue.AddRange(soundTracks);
        PlaySoundTrackFromQueue();
    }
    private void PlaySoundTrackFromQueue()
    {
        if (musicManagerQueue.Count == 0) return;
        MusicSoundTrack soundTrack = musicManagerQueue[0];
        musicManagerQueue.RemoveAt(0);
        PlaySoundTrack(soundTrack);
    }
    private void PlaySoundTrack(MusicSoundTrack soundTrack)
    {
        musicAudioSource.clip = soundTrack.audioClip;
        musicAudioSource.volume = soundTrack.volume;
        musicAudioSource.Play();
    }
    public void Pause()
    {
        musicAudioSource.Pause();
    }
    public void UnPause()
    {
        musicAudioSource.UnPause();
    }
    public void Stop()
    {
        musicAudioSource.Stop();
    }
}
