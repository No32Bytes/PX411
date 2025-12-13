using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 14f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckLength = 0.3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    private CharacterController characterController;
    private InputAction movementAction;
    private InputAction jumpAction;
    private Vector3 gravityVector;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        Enable();
    }
    void Update()
    {
        bool isGroundedCheck = IsGrounded();

        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        Vector3 movementVector = transform.right * movementInput.x;
        movementVector += transform.forward * movementInput.y;
        characterController.Move(movementSpeed * Time.deltaTime * movementVector.normalized);
        
        if(!isGroundedCheck)
            gravityVector.y += gravity * Time.deltaTime;
        if(isGroundedCheck && gravityVector.y < 0)
            gravityVector.y = 0;
        if(isGroundedCheck && jumpAction.triggered)
            gravityVector.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);
        characterController.Move(gravityVector * Time.deltaTime);

    }
    bool IsGrounded()
    {
        Debug.DrawRay(transform.position,-transform.up,Color.red,groundCheckLength);
        return Physics.Raycast(transform.position,-transform.up,groundCheckLength,groundLayerMask);
    }
    public void Enable()
    {
        movementAction.Enable();
        jumpAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Disable()
    {
        movementAction.Disable();
        jumpAction.Disable();
        Cursor.lockState = CursorLockMode.None;
    }
};