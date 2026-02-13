using System;
using UnityEngine;
using UnityEngine.UI;

namespace Entity
{
    [Serializable]
    public class HealthBar
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float healthRegenRate = 1f;
        [SerializeField] private Slider healthSlider;
        public float CurrentHealth { get; private set; }
        public delegate void OnDeath();
        private OnDeath onDeathCallback = null;
        public HealthBar()
        {
            CurrentHealth = maxHealth;
        }
        public void Update()
        {
            if (CurrentHealth <= 0)
            {
                onDeathCallback?.Invoke();
                return;
            }

            healthSlider.maxValue = maxHealth;
            float currentHealthRegenerated = healthRegenRate * Time.deltaTime;
            if (CurrentHealth + currentHealthRegenerated >= maxHealth)
                CurrentHealth = maxHealth;
            else
                CurrentHealth += currentHealthRegenerated;
            healthSlider.value = CurrentHealth;
        }
        public void SetOnDeathCallback(OnDeath onDeathCallback)
        {
            this.onDeathCallback = onDeathCallback;
        }
        public void ReduceHealth(float damageAmount)
        {
            if (CurrentHealth - damageAmount >= 0)
                CurrentHealth -= damageAmount;
            else
                CurrentHealth = 0;
        }
        public void IncreaseHealth(float healAmount)
        {
            if (CurrentHealth + healAmount >= maxHealth)
                CurrentHealth += healAmount;
            else
                CurrentHealth = maxHealth;
        }
    };
};