using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundTrack")]
public class SoundTrack : ScriptableObject
{
    [SerializeField] private string soundTrackId;
    [SerializeField] private string soundTrackGroup;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1.0f;
    public string GetSoundTrackId() { return soundTrackId; }
    public string GetSoundTrackGroup() { return soundTrackGroup; }
    public void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}