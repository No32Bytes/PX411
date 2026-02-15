using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private LayerMask playerInteractionLayerMask;
    [SerializeField] private float maxItemInteractionDistance = 1f;
    [SerializeField] private float interactionCooldownSeconds = 1f;
    private float xRotation = 0f;
    private InputAction lookAction;
    private InputAction interactAction;
    private float lastInteractionTimer = 0f;
    void Start()
    {
        playerRef.playerCamera.enabled = true;
        lookAction = InputSystem.actions.FindAction("Look");
        interactAction = InputSystem.actions.FindAction("Interact");
        lookAction.Enable();
    }

    void Update()
    {
        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        lookVector *= GlobalDataStore.GetSettingsData().mouseSensitivity * Time.deltaTime;

        playerRef.playerBody.Rotate(Vector3.up * lookVector.x);

        xRotation -= lookVector.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerRef.playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        HandlePlayerLooking();
    }
    private void HandlePlayerLooking()
    {
        Ray raycast = new(playerRef.playerCamera.transform.position, playerRef.playerCamera.transform.forward);
        //Debug.DrawRay(raycast.origin, raycast.direction, Color.red, maxItemInteractionDistance);
        if (!Physics.Raycast(raycast, out RaycastHit raycastHit, Mathf.Infinity, playerInteractionLayerMask) || !(raycastHit.distance < maxItemInteractionDistance))
        {
            return;
        }
        HandlePlayerLookingRaycastHit(raycastHit);
        return;
    }
    private void HandlePlayerLookingRaycastHit(RaycastHit raycastHit)
    {
        GameObject hitObject = raycastHit.transform.gameObject;
        if (hitObject.TryGetComponent(out ItemEntity itemEntity))
        {
            if(InteractWithCooldown())
                itemEntity.PickupItem();
        }
    }
    private bool InteractWithCooldown()
    {
        lastInteractionTimer += Time.fixedDeltaTime;
        if(lastInteractionTimer < interactionCooldownSeconds)
            return false;

        if (interactAction.IsPressed())
        {
            lastInteractionTimer = 0;
            return true;
        }
        return false;
    }
}
