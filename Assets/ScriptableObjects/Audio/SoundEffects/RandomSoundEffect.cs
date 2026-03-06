using UnityEngine;

[CreateAssetMenu(menuName = "Audio/RandomSoundEffect")]
public class RandomSoundEffect : BaseSoundEffect
{
    [SerializeField] private AudioClip[] audioClip;
    [SerializeField] private float volume;
    [SerializeField] private AudioUtil.RandomRange pitch;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip[Random.Range(0, audioClip.Length)];
        audioSource.pitch = pitch.GetRandom();
        audioSource.volume = volume;

        audioSource.Play();
    }
}