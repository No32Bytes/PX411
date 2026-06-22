using UnityEngine;

[RequireComponent(typeof(AudioSource))]
class PlayerRandomSound : MonoBehaviour
{
    [SerializeField] private BaseSoundEffect randomSounds;
    [SerializeField] private float maxDelaySeconds = 60f;
    [SerializeField] private float minDelaySeconds = 30f;
    private AudioSource audioSource;
    private float nextStartTime;
    private bool hasPlayed;
    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        nextStartTime = 0f;
        hasPlayed = true;
    }

    private void FixedUpdate()
    {
        if (audioSource.isPlaying)
            return;

        if (hasPlayed)
        {
            nextStartTime = Random.Range(minDelaySeconds, maxDelaySeconds);
            nextStartTime += Time.fixedTime;
            hasPlayed = false;
            return;
        }

        if (!hasPlayed && nextStartTime < Time.fixedTime)
        {
            AudioUtil.PlaySoundEffect(randomSounds, audioSource);
            hasPlayed = true;
        }
    }
};