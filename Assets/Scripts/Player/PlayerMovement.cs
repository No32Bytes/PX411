using System;
using Entity;
using UnityEngine;
using InputUtil;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float runMultiplicator = 1.5f;
    [SerializeField] private StaminaBar staminaBar = new();

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float pushPower = 2f;
    [SerializeField] private float inAirTimeLandSoundPlay = 2f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckLength = 0.1f;
    [Header("Sounds")]
    [SerializeField] private BaseSoundEffect walkSound;
    [SerializeField] private BaseSoundEffect runningSound;
    [SerializeField] private BaseSoundEffect jumpingSound;
    [SerializeField] private BaseSoundEffect landingSound;

    private CharacterController characterController;
    public CharacterController PlayerMovementController => characterController;
    private InputHandler movementAction, jumpAction, runAction;
    private Vector3 gravityVector;
    private AudioSource audioSource;
    enum MovementState
    {
        None,
        Walking,
        Running,
        Jumping,
        inAir,
        Landing,
    };
    MovementState movementState;
    MovementState lockedState;
    float inAir;

    public Transform MoveTransform => transform;

    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementAction = new("Move");
        jumpAction = new("Jump");
        runAction = new("Sprint");
        inAir = 0f;
    }
    void Update()
    {
        staminaBar.Update();

        bool isGroundedCheck = IsGrounded();

        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        Vector3 movementVector = transform.right * movementInput.x;
        movementVector += transform.forward * movementInput.y;

        if (runAction.IsPressed() && staminaBar.CurrentStamina > 0)
        {
            characterController.Move(movementSpeed * runMultiplicator * Time.deltaTime * movementVector.normalized);

            ReducePlayerStamina(1);
            movementState = MovementState.Running;
        }
        else
        {
            characterController.Move(movementSpeed * Time.deltaTime * movementVector.normalized);
            movementState = MovementState.Walking;
        }

        if (movementVector.magnitude < 0.1)
            movementState = MovementState.None;

        if (!isGroundedCheck)
        {
            gravityVector.y += gravity * Time.deltaTime;
            inAir += Time.deltaTime;
            movementState = MovementState.inAir;
        }
        if (isGroundedCheck && gravityVector.y < 0)
            gravityVector.y = 0;

        if (inAir > inAirTimeLandSoundPlay && isGroundedCheck)

            lockedState = MovementState.Landing;

        if (isGroundedCheck)
            inAir = 0f;

        if (isGroundedCheck && jumpAction.IsPressed())
        {
            gravityVector.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);
            lockedState = MovementState.Jumping;
        }


        characterController.Move(gravityVector * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        HandleMovementState();
    }

    private void HandleMovementState()
    {
        if (audioSource.isPlaying && lockedState == MovementState.None)
            return;
        if (lockedState != MovementState.None)
        {
            movementState = lockedState;
            lockedState = MovementState.None;
            audioSource.Stop();
        }

        BaseSoundEffect toPlay;
        switch (movementState)
        {
            case MovementState.Walking:
                toPlay = walkSound;
                break;
            case MovementState.Running:
                toPlay = runningSound;
                break;
            case MovementState.Jumping:
                toPlay = jumpingSound;
                break;
            case MovementState.Landing:
                toPlay = landingSound;
                break;
            default:
                return;
        }

        lockedState = MovementState.None;
        AudioUtil.PlaySoundEffect(toPlay, audioSource);
    }


    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3)
            return;

        Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * pushPower;

    }
    public bool IsGrounded()
    {
        if (characterController.isGrounded)
            return true;

        return Physics.Raycast(transform.position, -transform.up, groundCheckLength, groundLayerMask);
    }

    public void ReducePlayerStamina(float staminaMinusAmount) { staminaBar.ReduceStamina(staminaMinusAmount); }
    public void IncreasePlayerStamina(float staminaPlusAmount) { staminaBar.IncreaseStamina(staminaPlusAmount); }

};