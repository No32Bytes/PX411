using UnityEngine;

public abstract class BaseSoundEffect : ScriptableObject
{
    public abstract void Play(AudioSource audiosource);
}