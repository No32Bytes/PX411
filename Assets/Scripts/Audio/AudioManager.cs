using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("AudioManager")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundMixerGroup;
    [SerializeField] private List<SoundTrack> soundTrackStore = new();
    public List<SoundTrack> InternalGetSoundTrackStore => soundTrackStore;
    public AudioMixerGroup SoundMixerGroup => soundMixerGroup;
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;
    private bool isPlaying = false;
    private string currentSoundTrackId = "";
    private string currentSoundTrackGroup = "";
    private readonly List<SoundTrack> soundTrackQueue = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initalize();
    }
    private void Initalize()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private void FixedUpdate()
    {
        if (!isPlaying)
            return;

        if (audioSource.isPlaying)
            return;

        if (soundTrackQueue.Count != 0)
        {
            PlaySoundTrackFromQueue();
            return;
        }
        HandleEmptySoundTrackQueue();
    }
    private void PlaySoundTrackFromQueue()
    {
        soundTrackQueue[0].Play(audioSource);
        soundTrackQueue.RemoveAt(0);
    }
    private void HandleEmptySoundTrackQueue()
    {
        if (!string.IsNullOrEmpty(currentSoundTrackId))
        {
            soundTrackQueue.Add(soundTrackStore.Find(track => track.GetSoundTrackId() == currentSoundTrackId));
            return;
        }

        List<SoundTrack> soundTracks = soundTrackStore.FindAll(track => track.GetSoundTrackGroup() == currentSoundTrackGroup);
        if (soundTracks.Count == 0)
        {
            isPlaying = false;
            return;
        }

        soundTrackQueue.AddRange(soundTracks);
    }
    public void PlaySoundTrackId(SoundTrack soundTrack)
    {
        currentSoundTrackId = soundTrack.GetSoundTrackId();
        soundTrackQueue.Clear();
        audioSource.Stop();
        isPlaying = true;
    }
    public void PlaySoundTrackGroup(SoundTrack soundTrack)
    {
        currentSoundTrackGroup = soundTrack.GetSoundTrackGroup();
        soundTrackQueue.Clear();
        audioSource.Stop();
        isPlaying = true;
    }
    public void Pause()
    {
        isPlaying = false;
        audioSource.Pause();
    }
    public void Resume()
    {
        isPlaying = true;
        audioSource.UnPause();
    }
}