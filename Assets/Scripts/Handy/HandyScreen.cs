using UnityEngine;

public class HandyScreen : MonoBehaviour
{
    [SerializeField] private GameObject cheatMenuButton;
    private bool cheatMenuEnabled = false;
    private float timer;
    public void OnReturnButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }
    public void Start()
    {
        DebugDev.DebugFunction.RegisterDebugCallback(ToggleCheatMenu);
        cheatMenuButton.SetActive(cheatMenuEnabled);
    }
    public void ToggleCheatMenu()
    {
        if(timer + 1f > Time.unscaledTime)
            return;

        timer = Time.unscaledTime;
        cheatMenuEnabled = !cheatMenuEnabled;
        cheatMenuButton.SetActive(cheatMenuEnabled);
    }
}