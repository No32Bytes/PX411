using Entity;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeacherMullerEntity : BaseEntity
{
    private CharacterController characterController;
    [SerializeField] private HealthBar healthBar = new();
    private void Start()
    {
        healthBar.SetOnDeathCallback(OnDeath);
        
        characterController = GetComponent<CharacterController>();

    }
    private void Update()
    {
        Damage(0.5f);
        healthBar.Update();
    }

    private void OnDeath()
    {
        
    }

    public void Damage(float damageAmount) { healthBar.ReduceHealth(damageAmount); }
    public void Heal(float healAmount) { healthBar.IncreaseHealth(healAmount); }
}