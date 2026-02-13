using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Serializable]
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
    private static readonly Queue<MusicSoundTrack> musicManagerQueue = new();
    public static AudioManager Instance { get; private set; }
    private static AudioSource musicAudioSource;
    private static bool isPlaying = true;
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
        if (!isPlaying) return;

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
        foreach (MusicSoundTrack musicSoundTrack in Array.FindAll(musicSoundTrackStore, m => m.soundTrackGroup == CurrentDefaultGroupName))
            musicManagerQueue.Enqueue(musicSoundTrack);

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
        foreach (MusicSoundTrack soundtrack in soundTracks)
            musicManagerQueue.Enqueue(soundtrack);
        PlaySoundTrackFromQueue();
    }
    private void PlaySoundTrackFromQueue()
    {
        if (musicManagerQueue.Count == 0) return;
        MusicSoundTrack soundTrack = musicManagerQueue.Dequeue();
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
        isPlaying = false;
        musicAudioSource.Pause();
    }
    public void UnPause()
    {
        isPlaying = true;
        musicAudioSource.UnPause();
    }
    public void Stop()
    {
        musicAudioSource.Stop();
    }
}
