using UnityEngine;

public class HandyScreen : MonoBehaviour
{
    public void OnReturnButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }
}