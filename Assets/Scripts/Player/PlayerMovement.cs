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


    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    private CharacterController characterController;
    private InputHandler movementAction, jumpAction, runAction;
    private Vector3 gravityVector;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementAction = new("Move");
        jumpAction = new("Jump");
        runAction = new("Sprint");
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
        }
        else
        {
            characterController.Move(movementSpeed * Time.deltaTime * movementVector.normalized);
        }

        if (!isGroundedCheck)
            gravityVector.y += gravity * Time.deltaTime;
        if (isGroundedCheck && gravityVector.y < 0)
            gravityVector.y = 0;
        if (isGroundedCheck && jumpAction.IsPressed())
            gravityVector.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);
        characterController.Move(gravityVector * Time.deltaTime);

    }
    bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public void ReducePlayerStamina(float staminaMinusAmount) { staminaBar.ReduceStamina(staminaMinusAmount); }
    public void IncreasePlayerStamina(float staminaPlusAmount) { staminaBar.IncreaseStamina(staminaPlusAmount); }

};