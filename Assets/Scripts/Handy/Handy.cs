using InputUtil;
using UnityEngine;

public class Handy : MonoBehaviour
{
    [SerializeField] private Light flashLight;
    [SerializeField] private float flashLightToggleActionCooldownSeconds = 0.2f;
    [SerializeField] private float toggleViewHandyActionCooldownSeconds = 0.2f;
    private InputHandlerCooldown flashLightToggleAction, toggleViewHandy;
    private bool handyOpen = false;
    public void Start()
    {
        flashLightToggleAction = new("FlashLightToggle", flashLightToggleActionCooldownSeconds);
        toggleViewHandy = new("ToggleViewHandy", toggleViewHandyActionCooldownSeconds,InputHandlerCooldown.CooldownType.TimeUnscaled);
    }
    public void Update()
    {
        if (flashLightToggleAction.InteractWithCooldown())
            flashLight.enabled = !flashLight.enabled;
        
        if (toggleViewHandy.InteractWithCooldown())
            ToggleViewHandy();
    }

    private void ToggleViewHandy()
    {
        GlobalDataStore.GetStateManager().player.playerReference.HandySetActive(handyOpen);
        handyOpen = !handyOpen;
    }
}