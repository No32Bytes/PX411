using UnityEngine;

public class Handy : MonoBehaviour
{
    [SerializeField] private Light flashLight;

    
    public void SetFlashLightState(bool enabledState)
    {
        flashLight.enabled = enabledState;
    }
    public void ToggleFlashLight()
    {
        flashLight.enabled = !flashLight.enabled;
    }
}