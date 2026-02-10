using System;
using UnityEngine;
using UnityEngine.UI;

namespace Entity
{
    [Serializable]
    public class StaminaBar
    {
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegenRate = 1f;
        [SerializeField] private Slider staminaSlider;

        [SerializeField] private float noSprintTime = 250f;
        private bool canRegen;
        private float regenTimer;

        public float CurrentStamina { get; private set;}
        public StaminaBar()
        {
            CurrentStamina = maxStamina;
            regenTimer = 0f;
            canRegen = true;
        }
        public void Update()
        {
            staminaSlider.maxValue = maxStamina;

            if (CurrentStamina <= 0 && canRegen == true)
            {
                canRegen = false;
            }
            if (canRegen)
            {
                float currentStaminaRegenerated = staminaRegenRate * Time.deltaTime;
                if (CurrentStamina + currentStaminaRegenerated >= maxStamina)
                    CurrentStamina = maxStamina;
                else
                    CurrentStamina += currentStaminaRegenerated;
                staminaSlider.value = CurrentStamina;
            }
            else
            {
                regenTimer += Time.deltaTime * 100;
                if (regenTimer > noSprintTime)
                {
                    regenTimer = 0f;
                    canRegen = true;
                    CurrentStamina++;
                }
            }
        }
        public void ReduceStamina(float damageAmount)
        {
            if (CurrentStamina - damageAmount >= 0)
                CurrentStamina -= damageAmount;
            else
                CurrentStamina = 0;
        }
        public void IncreaseStamina(float healAmount)
        {
            if (CurrentStamina + healAmount >= maxStamina)
                CurrentStamina += healAmount;
            else
                CurrentStamina = maxStamina;
        }

        public float GetCurrentStamina()
        {
            return CurrentStamina;
        }

        
    };
};