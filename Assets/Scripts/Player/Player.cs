using UnityEngine;
using Entity;
public class Player : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar = new();
    private void Start()
    {
        healthBar.SetOnDeathCallback(OnPlayerDeath);
    }
    private void Update()
    {
        healthBar.Update();
    }
    public void DamagePlayer(float damageAmount) { healthBar.ReduceHealth(damageAmount); }
    public void HealPlayer(float healAmount) { healthBar.IncreaseHealth(healAmount); }
    private void OnPlayerDeath()
    {
        return;
    }
}