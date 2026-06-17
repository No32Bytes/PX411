using UnityEngine;
using InputUtil;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private LayerMask playerInteractionLayerMask;
    [SerializeField] private float maxItemInteractionDistance = 1f;
    [SerializeField] private float interactActionCooldownS;
    [SerializeField] private BaseSoundEffect interactActionSound;
    [SerializeField] private float holdActionCooldownS;
    [SerializeField] private AnimationParamterInfo interactAnimation;
    private InputHandlerCooldown interactAction, holdAction;
    private float xRotation = 0f;
    private InputHandler lookAction;
    private AudioSource audioSource;
    private bool entityInformationViewFrameDisable;
    void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        GlobalDataStore.GetStateManager().playerState.playerLook = this;
        entityInformationViewFrameDisable = false;
    }
    void Start()
    {
        lookAction = new("Look");
        interactAction = new("Interact", interactActionCooldownS);
        holdAction = new("Hold", holdActionCooldownS);

        playerRef.playerCamera.enabled = true;
        lookAction.Enable();
    }

    void Update()
    {
        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        lookVector *= GlobalDataStore.GetSettingsData().mouseSensitivity * Time.deltaTime;

        playerRef.playerBody.Rotate(Vector3.up * lookVector.x);

        xRotation -= lookVector.y;
        xRotation = Mathf.Clamp(xRotation, -83f, 83f);
        playerRef.playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerRef.overlayFPS.transform.localRotation = playerRef.playerCamera.transform.localRotation;

        HandlePlayerLooking();
    }
    private void HandlePlayerLooking()
    {
        Ray raycast = new(playerRef.playerCamera.transform.position, playerRef.playerCamera.transform.forward);
        GameObject hitObject = null;
        if (Physics.Raycast(raycast, out RaycastHit raycastHit, Mathf.Infinity, playerInteractionLayerMask) || !(raycastHit.distance < maxItemInteractionDistance))
        {
            hitObject = raycastHit.transform.gameObject;
            HandlePlayerLookingRaycastHit(raycastHit);
        }

        if (holdAction.InteractWithCooldown() && EntityDraggable.IsEntitySelected())
            EntityDraggable.CurrentDraggedEntity.DeselectEntity();

        EntityInformationView entityInformationView = hitObject == null ? null : hitObject.transform.gameObject.GetComponent<EntityInformationView>();
        if (entityInformationViewFrameDisable)
        {
            entityInformationView = null;
            entityInformationViewFrameDisable = false;
        }
        EntityInformationView.SelectEntity(entityInformationView);
    }
    private void HandlePlayerLookingRaycastHit(RaycastHit raycastHit)
    {
        GameObject hitObject = raycastHit.transform.gameObject;
        if (hitObject.TryGetComponent(out BaseEntity baseEntity))
        {
            if (interactAction.InteractWithCooldown())
            {
                AudioUtil.PlaySoundEffect(interactActionSound, audioSource);
                interactAnimation.SetTrigger();
                baseEntity.EntityInteraction();
                entityInformationViewFrameDisable = true;
                return;
            }
        }

        if (!EntityDraggable.IsEntitySelected())
        {
            if (hitObject.TryGetComponent(out EntityDraggable entityDraggable))
            {
                if (holdAction.InteractWithCooldown())
                    entityDraggable.SelectEntity(playerRef.playerCamera);
            }
        }
    }

    public void HandlePlayerLookAttack(float damageAmount)
    {
        Ray raycast = new(playerRef.playerCamera.transform.position, playerRef.playerCamera.transform.forward);
        if (!Physics.Raycast(raycast, out RaycastHit raycastHit, Mathf.Infinity, playerInteractionLayerMask) || !(raycastHit.distance < maxItemInteractionDistance))
            return;

        GameObject hitObject = raycastHit.transform.gameObject;
        if (hitObject.TryGetComponent(out EnemeyEntity enemeyEntity))
            enemeyEntity.EntityDamage(damageAmount);

    }
}
