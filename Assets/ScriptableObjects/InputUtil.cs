using UnityEngine;
using UnityEngine.InputSystem;


namespace InputUtil
{

    public class InputHandler
    {
        public readonly InputAction inputAction;
        public InputHandler(string actionNameOrId)
        {
            inputAction = InputSystem.actions.FindAction(actionNameOrId, true);
        }
        public TValue ReadValue<TValue>() where TValue : struct
        {
            return inputAction.ReadValue<TValue>();
        }
        public bool IsPressed()
        {
            return inputAction.IsPressed();
        }
        public void Enable() { inputAction.Enable(); }
        public void Disable() { inputAction.Disable(); }
    }
    public class InputHandlerCooldown : InputHandler
    {
        public float InputCooldownSeconds { get; private set; }
        private float lastInteractionTimer = Mathf.NegativeInfinity;
        public InputHandlerCooldown(string actionNameOrId, float inputCooldownSeconds)
        : base(actionNameOrId)
        {
            InputCooldownSeconds = inputCooldownSeconds;
        }
        public bool InteractWithCooldown()
        {
            if (Time.time - lastInteractionTimer < InputCooldownSeconds)
                return false;

            if (!inputAction.IsPressed())
                return false;

            lastInteractionTimer = Time.time;
            return true;
        }
    }
}