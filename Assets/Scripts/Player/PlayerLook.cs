using UnityEngine;
using InputUtil;
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private LayerMask playerInteractionLayerMask;
    [SerializeField] private float maxItemInteractionDistance = 1f;
    [SerializeField] private float interactionCooldownSeconds = 1f;
    private float xRotation = 0f;
    private InputHandler lookAction;
    private InputHandlerCooldown interactAction;
    void Start()
    {
        lookAction = new("Look");
        interactAction = new("Interact",interactionCooldownSeconds);
        
        playerRef.playerCamera.enabled = true;
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
        HandlePlayerLooking();
    }
    private void HandlePlayerLooking()
    {
        Ray raycast = new(playerRef.playerCamera.transform.position, playerRef.playerCamera.transform.forward);
        if (!Physics.Raycast(raycast, out RaycastHit raycastHit, Mathf.Infinity, playerInteractionLayerMask) || !(raycastHit.distance < maxItemInteractionDistance))
        {
            return;
        }
        
        HandlePlayerLookingRaycastHit(raycastHit);
        
    }
    private void HandlePlayerLookingRaycastHit(RaycastHit raycastHit)
    {
        GameObject hitObject = raycastHit.transform.gameObject;
        if (hitObject.TryGetComponent(out BaseEntity baseEntity))
        {
            if (interactAction.InteractWithCooldown())
                baseEntity.EntityInteraction();
        }
    }
}
