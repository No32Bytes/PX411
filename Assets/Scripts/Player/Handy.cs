using InputUtil;
using UnityEngine;

public class Handy : MonoBehaviour
{
    [SerializeField] private Light flashLight;
    [SerializeField] private float flashLightToggleActionCooldownSeconds = 0.2f;
    private InputHandlerCooldown flashLightToggleAction;
    public void Start()
    {
        flashLightToggleAction = new("FlashLightToggle",flashLightToggleActionCooldownSeconds);
    }
    public void Update()
    {
        if (flashLightToggleAction.InteractWithCooldown())
        {
            flashLight.enabled = !flashLight.enabled;   
        }
    }
}