using Entity;
using UnityEngine;
using System;
using InputUtil;

[RequireComponent(typeof(CharacterController))]
public class TeacherMullerEntity : BaseEntity
{
    private CharacterController characterController;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckLength = 0.3f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 gravityVector;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Player playerPlayer;

    [SerializeField] private DamagePlayer ball1;
    [SerializeField] private DamagePlayer ball2;
    [SerializeField] private DamagePlayer ball3;


    private float attackGroundTimer = 0f;

    private float attackChargeTimer = 0f;
    private float chargeSpeed = 0.8f;
    private Vector3 chargeRichtung;
    private bool chargeState = false;

    private Vector3 throwDirection, throwDirection2, throwDirection3;
    private float throwSpeed = 0.05f;

    private bool attacking = false;

    //Attacks Timer
    private float startTimer;
    private bool starttiming;
    [SerializeField] private float startTimerMax = 2f;

    private void Start()
    {
        healthBar.SetOnDeathCallback(OnDeath);

        characterController = GetComponent<CharacterController>();

        StartAttackTimerActivate(startTimerMax);
    }


    private void Update()
    {

        bool isGroundedCheck = IsGrounded();
        healthBar.Update();

        if (!isGroundedCheck)
            gravityVector.y += gravity * Time.deltaTime;
        if (isGroundedCheck && gravityVector.y < 0)
            gravityVector.y = 0;
        characterController.Move(gravityVector * Time.deltaTime);




        //states for the attacks, every frame active, only activated by using the corresponding activate fuctions
        AttackGround();
        ChargeAttack();
        AcidAttack();

        //Timer for Attacks
        StartAttackTimer();

    }

    public void StartAttackTimerActivate(float timer)
    {
        startTimer = timer;
        starttiming = true;
    }

    public void StartAttackTimer()
    {
        startTimer -= Time.deltaTime;
        if (startTimer <= 0 && starttiming == true)
        {
            StartAttack();
            starttiming = false;
            StartAttackTimerActivate(startTimerMax);
        }
    }


    public void StartAttack()
    {
        int x = UnityEngine.Random.Range(1, 4);
        Damage(20);
        if (x == 3)
        {
            AttackGroundActivate();
        }
        else if (x == 2)
        {
            ChargeAttackActivate();
        }
        else if (x == 1)
        {
            AcidAttackActivate();
        }
    }

    //ground attack; activate ground attack
    public void AttackGroundActivate()
    {
        Debug.Log("GroundAttack");
        attackGroundTimer = 6f;
    }
    //ground attack; ground attack state
    public void AttackGround()
    {
        attackGroundTimer -= Time.deltaTime * 7;
        if (attackGroundTimer > 2f)
        {
            //Debug.Log(attackGroundTimer);
        }

        if (attackGroundTimer > 0f && attackGroundTimer < 2f)
        {
            if (playerMovement.IsGrounded())
            {
                playerPlayer.Damage(10);
                attackGroundTimer = 0;
            }
        }
    }


    //Charge attack; activate the charge attack state
    public void ChargeAttackActivate()
    {
        Debug.Log("ChargeAttack");
        attackChargeTimer = 12f;
        Vector3 playerPosition = playerMovement.moveTransform.position;

        chargeRichtung = (playerPosition - transform.position).normalized;
        chargeRichtung.y = 0;
    }
    //Charge attack; charge attack state
    public void ChargeAttack()
    {
        attackChargeTimer -= Time.deltaTime * 7;
        if (attackChargeTimer > 1f)
        {
            //Debug.Log(attackChargeTimer);
            chargeState = false;
        }
        else if (attackChargeTimer > 0f && attackChargeTimer < 1f)
        {
            chargeState = true;
            characterController.Move(chargeRichtung * chargeSpeed);
        }
        else if (attackChargeTimer < 0f)
        {
            chargeState = false;
        }

    }

    //Charge attack; collision with player and damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && chargeState == true)
        {
            playerPlayer.Damage(50);
            chargeState = false;
            attackChargeTimer = 0f;
        }

    }


    public void AcidAttackActivate()
    {
        Debug.Log("AcidAttack");
        Vector3 playerPosition = playerMovement.moveTransform.position;

        throwDirection = (playerPosition - transform.position).normalized;
        throwDirection.y -= throwDirection.y;

        Vector3 playerPosition2 = playerPosition;
        playerPosition2.x = 2f * playerPosition2.x;


        Vector3 playerPosition3 = playerPosition;
        playerPosition3.x = -playerPosition3.x;

        throwDirection2 = (playerPosition2 - transform.position).normalized;
        throwDirection2.y -= throwDirection2.y;

        throwDirection3 = (playerPosition3 - transform.position).normalized;
        throwDirection3.y -= throwDirection3.y;

        ball1.activateTimer();
        ball2.activateTimer();
        ball3.activateTimer();

    }


    //Acid attack;  the acid attack state
    public void AcidAttack()
    {
        if (ball1.CanBallDamage())
        {
            ball1.transform.Translate(throwDirection * throwSpeed);
        }
        if (ball2.CanBallDamage())
        {
            ball2.transform.Translate(throwDirection2 * throwSpeed);
        }
        if (ball3.CanBallDamage())
        {
            ball3.transform.Translate(throwDirection3 * throwSpeed);
        }
    }



    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, groundCheckLength, groundLayerMask);
    }

    public Transform mullerCoordinations()
    {
        return characterController.transform;
    }

    private void OnDeath()
    {
        //Irgendein Objekt will noch auf Herr Muller zugreifen, nachdem er tot ist. 
        //Wird als Fehler angezeigt, scheint das Spiel allerdings nicht zu st�ren.
        this.DestroyEntity();
    }


    public void Damage(float damageAmount) { healthBar.ReduceHealth(damageAmount); }
    public void Heal(float healAmount) { healthBar.IncreaseHealth(healAmount); }
}