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
    public string CurrentSoundTrackId { get; set; } = "";
    public string CurrentSoundTrackGroup { get; set; } = "";
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


    private void Update()
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
        if (!string.IsNullOrEmpty(CurrentSoundTrackId))
        {
            soundTrackQueue.Add(soundTrackStore.Find(track => track.GetSoundTrackId() == CurrentSoundTrackId));
            return;
        }

        List<SoundTrack> soundTracks = soundTrackStore.FindAll(track => track.GetSoundTrackGroup() == CurrentSoundTrackGroup);
        if (soundTracks.Count == 0)
        {
            isPlaying = false;
            return;
        }

        soundTrackQueue.AddRange(soundTracks);
    }
    public void PlaySoundTrackId(SoundTrack soundTrack)
    {
        PlaySoundTrackIdString(soundTrack.GetSoundTrackId());
    }
    public void PlaySoundTrackGroup(SoundTrack soundTrack)
    {
        PlaySoundTrackGroupString(soundTrack.GetSoundTrackGroup());
    }
    public void PlaySoundTrackIdString(string soundTrackId)
    {
        CurrentSoundTrackId = soundTrackId;
        soundTrackQueue.Clear();
        audioSourceMusic.Stop();
        isPlaying = true;
    }
    public void PlaySoundTrackGroupString(string soundTrackGroup)
    {
        CurrentSoundTrackGroup = soundTrackGroup;
        soundTrackQueue.Clear();
        audioSourceMusic.Stop();
        isPlaying = true;
    }
    public void ReloadAfterManualSet()
    {
        isPlaying = true;
        audioSourceMusic.Stop();
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

    public float GetCurrentSoundTrackTime()
    {
        return audioSourceMusic.time;
    }
    public void SetCurrentSoundTrackTime(float time)
    {
        audioSourceMusic.time = time;
    }
}