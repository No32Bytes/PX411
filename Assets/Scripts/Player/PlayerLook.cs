using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask playerInteractionLayerMask;
    [SerializeField] private float maxItemInteractionDistance = 1f;
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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void FixedUpdate()
    {
        Ray raycast = new(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(raycast.origin, raycast.direction, Color.red, maxItemInteractionDistance);
        if (!Physics.Raycast(raycast, out RaycastHit raycastHit, Mathf.Infinity, playerInteractionLayerMask) || raycastHit.distance > maxItemInteractionDistance)
        {
            // Nothing hit 
            return;
        }
        // Hit
        return;
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
