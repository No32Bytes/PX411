using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SimpleSoundEffect")]
public class SimpleSoundEffect : SoundEffect
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.volume = volume;

        audioSource.Play();
    }
}