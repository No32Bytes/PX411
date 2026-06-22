using UnityEngine;

class BossArenaCloseDoor : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] BaseSoundEffect doorCloseSound;
    private bool done = false;
    private void OnTriggerEnter(Collider other)
    {
        if (done)
            return;
        done = true;
        door.SetActive(true);
        AudioUtil.PlaySoundEffect(doorCloseSound, GlobalDataStore.GetStateManager().playerState.player.OverrideDamageAudioSource);
    }
}