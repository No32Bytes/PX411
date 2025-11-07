using UnityEngine;

[CreateAssetMenu(menuName = "Audio/RandomSoundEffect")]
public class RandomSoundEffect : SoundEffect
{
    [SerializeField] private AudioClip[] audioClip;
    [SerializeField] private float volume;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip[UnityEngine.Random.Range(0, audioClip.Length)];
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.volume = volume;

        audioSource.Play();
    }
}