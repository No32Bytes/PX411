using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera playerCamera;
    private float xRotation = 0f;
    private InputAction lookAction;
    void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        lookAction.Enable();
    }

    void Update()
    {
        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        lookVector *= GlobalDataStore.GetSettingsData().mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * lookVector.x);

        xRotation -= lookVector.y;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
    }
    public void Enable()
    {
        lookAction.Enable();
    }
    public void Disable()
    {
        lookAction.Disable();
    }
}
