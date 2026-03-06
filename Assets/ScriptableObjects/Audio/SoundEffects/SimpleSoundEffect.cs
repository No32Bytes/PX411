using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SimpleSoundEffect")]
public class SimpleSoundEffect : BaseSoundEffect
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume;
    [SerializeField] private AudioUtil.RandomRange pitch;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = pitch.GetRandom();
        audioSource.volume = volume;

        audioSource.Play();
    }
}