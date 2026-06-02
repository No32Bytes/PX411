using System.Collections.Generic;
using Entity;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[DefaultExecutionOrder(1)]
public class TeacherMullerEntity : EnemeyEntity
{
    private CharacterController characterController;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private float gravity = -9.81f;
    private Vector3 gravityVector;

    private PlayerMovement playerMovement;
    private Player playerPlayer;

    [SerializeField] private BallOfDoom[] balls;
    private readonly List<Vector3> throwDirections = new();
    private float attackGroundTimer = 0f;

    private float attackChargeTimer = 0f;
    private float chargeSpeed = 0.8f;
    private Vector3 chargeRichtung;
    private bool chargeState = false;
    [SerializeField] private float throwSpeed = 0.05f;

    //Attacks Timer
    private float startTimer;
    private bool starttiming;
    [SerializeField] private float startTimerMax = 2f;

    private void Awake()
    {
        playerPlayer = GlobalDataStore.GetStateManager().playerState.player;
        if (playerPlayer == null)
            throw new System.Exception("Player is null");

        playerMovement = playerPlayer.GetComponent<PlayerMovement>();
        if (playerMovement == null)
            throw new System.Exception("PlayersMovement is null");
    }
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
        int x = Random.Range(1, 4);
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
        //Debug.Log("GroundAttack");
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
        //Debug.Log("ChargeAttack");
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
        //Debug.Log("AcidAttack");
        Vector3 playerPosition = playerMovement.moveTransform.position;

        throwDirections.Clear();
        Vector3 throwDirection;
        throwDirection = (playerPosition - transform.position).normalized;
        throwDirection.y -= throwDirection.y;
        throwDirections.Add(throwDirection);

        Vector3 playerPosition2 = playerPosition;
        playerPosition2.x = 2f * playerPosition2.x;


        Vector3 playerPosition3 = playerPosition;
        playerPosition3.x = -playerPosition3.x;

        throwDirection = (playerPosition2 - transform.position).normalized;
        throwDirection.y -= throwDirection.y;
        throwDirections.Add(throwDirection);

        throwDirection = (playerPosition3 - transform.position).normalized;
        throwDirection.y -= throwDirection.y;
        throwDirections.Add(throwDirection);

        foreach (var ball in balls)
            ball.ActiveTimer();

    }


    //Acid attack;  the acid attack state
    public void AcidAttack()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            BallOfDoom ball = balls[i];
            if (ball.CanBallDamage())
                ball.transform.Translate(throwDirections[i] * throwSpeed);
        }
    }



    bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public Transform MullerTransform => characterController.transform;


    private void OnDeath()
    {
        Destroy(gameObject);
    }

    public override void EntityDamage(float amount)
    {
        healthBar.ReduceHealth(amount);
    }
    public override void EntityHeal(float amount)
    {
        healthBar.IncreaseHealth(amount);
    }
}