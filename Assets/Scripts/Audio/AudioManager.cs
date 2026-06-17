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
    private AudioSource audioSourceMusic;
    private AudioSource audioSourceSFX;
    public AudioSource GlobalSoundAudioSource => audioSourceSFX;
    private bool isPlaying = false;
    private string currentSoundTrackId = "";
    private string currentSoundTrackGroup = "";
    private readonly List<SoundTrack> soundTrackQueue = new();
    private void Awake()
    {
        audioSourceMusic = gameObject.AddComponent<AudioSource>();
        audioSourceMusic.outputAudioMixerGroup = musicMixerGroup;
        audioSourceMusic.spatialBlend = 0.0f;

        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        audioSourceSFX.outputAudioMixerGroup = soundMixerGroup;
        audioSourceSFX.spatialBlend = 0.0f;
    }

    private void FixedUpdate()
    {
        if (!isPlaying)
            return;

        if (audioSourceMusic.isPlaying)
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
        soundTrackQueue[0].Play(audioSourceMusic);
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
        audioSourceMusic.Stop();
        isPlaying = true;
    }
    public void PlaySoundTrackGroup(SoundTrack soundTrack)
    {
        currentSoundTrackGroup = soundTrack.GetSoundTrackGroup();
        soundTrackQueue.Clear();
        audioSourceMusic.Stop();
        isPlaying = true;
    }
    public void Pause()
    {
        isPlaying = false;
        audioSourceMusic.Pause();
    }
    public void Resume()
    {
        isPlaying = true;
        audioSourceMusic.UnPause();
    }
}