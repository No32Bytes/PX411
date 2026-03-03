using UnityEngine;

public class HandyScreen : MonoBehaviour
{
    public void OnReturnButtonOnClick()
    {
        GlobalDataStore.GetStateManager().player.playerReference.HandySetActive(false);
    }
}