using System;
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
        public enum CooldownType
        {
            Time,
            TimeFixed,
            TimeUnscaled,
            TimeFixedUnscaled
        }
        private readonly CooldownType cooldownType;
        private readonly float inputCooldownSeconds;
        private float lastInteractionTimer = Mathf.NegativeInfinity;

        public InputHandlerCooldown(string actionNameOrId, float inputCooldownSeconds, CooldownType cooldownType = CooldownType.Time)
        : base(actionNameOrId)
        {
            this.inputCooldownSeconds = inputCooldownSeconds;
            this.cooldownType = cooldownType;
        }
        public bool InteractWithCooldown()
        {
            if (GetCurrentTime() - lastInteractionTimer < inputCooldownSeconds)
                return false;

            if (!IsPressed())
                return false;

            lastInteractionTimer = GetCurrentTime();
            return true;
        }
        private float GetCurrentTime()
        {
            return cooldownType switch
            {
                CooldownType.TimeFixed => Time.fixedTime,
                CooldownType.TimeUnscaled => Time.unscaledTime,
                CooldownType.TimeFixedUnscaled => Time.fixedUnscaledTime,
                _ => Time.time,
            };
        }
    }

}