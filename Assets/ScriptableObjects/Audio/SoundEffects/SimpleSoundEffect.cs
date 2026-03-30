using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SimpleSoundEffect")]
public class SimpleSoundEffect : BaseSoundEffect
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioUtil.RandomRange pitch;
    public override void Play(AudioSource audioSource)
    {
        if(audioClip == null) return;

        audioSource.clip = audioClip;
        audioSource.pitch = pitch.GetRandom();
        audioSource.volume = volume;

        audioSource.Play();
    }
}