using System.Collections.Generic;
using Entity;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BossMuller : EnemeyEntity
{
    private CharacterController characterController;
    [SerializeField] private Canvas overlayMullerUI;
    [SerializeField] private HealthBar healthBar = new();
    [SerializeField] private float gravity = -9.81f;
    [Header("Sound")]
    [SerializeField] private BaseSoundEffect bossDamageSound;
    [SerializeField] private BaseSoundEffect bossChargeSound;
    [SerializeField] private BaseSoundEffect bossStompSound;
    private Vector3 gravityVector;

    private PlayerMovement playerMovement;
    private Player playerPlayer;

    [SerializeField] private BallOfDoom[] balls;
    private readonly List<Vector3> throwDirections = new();
    [SerializeField] private float attackGroundTimerStart = 6f;
    private bool attackGroundDidDamage;
    private float attackGroundTimer = 0f;
    [SerializeField] private ParticleSystem attackGround;
    private float attackChargeTimer = 0f;
    [SerializeField] private float chargeDamage = 50f;
    [SerializeField] private float groundDamage = 10f;
    private Vector3 chargeRichtung;
    private bool chargeState = false;
    private int chargeStep;
    [SerializeField] private int chargeStepAmount = 10;
    [SerializeField] private float throwSpeed = 0.05f;

    //Attacks Timer
    private float startTimer;
    private bool starttiming;
    [SerializeField] private float startTimerMax = 2f;
    [SerializeField] private float attackChargeTimerStart = 5f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
    }

    private void Start()
    {
        playerPlayer = GlobalDataStore.GetStateManager().playerState.player;
        playerMovement = playerPlayer.GetComponent<PlayerMovement>();
        overlayMullerUI.worldCamera = GlobalDataStore.GetStateManager().playerState.playerRef.playerOverlayCamera;

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

        ChargeAttackActivate();
        //states for the attacks, every frame active, only activated by using the corresponding activate fuctions


        //Timer for Attacks
        StartAttackTimer();

    }

    private void FixedUpdate()
    {
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
        startTimer -= Time.fixedDeltaTime;
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
        if (attackGroundTimer >= 0)
        {
            AcidAttackActivate();
            return;

        }
        //Debug.Log("GroundAttack");
        attackGroundDidDamage = false;
        attackGroundTimer = attackGroundTimerStart;
    }
    //ground attack; ground attack state
    public void AttackGround()
    {
        if (attackGroundTimer == attackGroundTimerStart)
        {
            AudioUtil.PlaySoundEffect(bossStompSound,audioSource); 
            attackGround.Play();
        }

        attackGroundTimer -= Time.fixedDeltaTime;
    }


    //Charge attack; activate the charge attack state
    public void ChargeAttackActivate()
    {
        if (attackChargeTimer >= 0)
            return;
        Vector3 playerPosition = playerMovement.MoveTransform.position;
        chargeRichtung = playerPosition - transform.position;
        chargeRichtung.y = 0;
        chargeRichtung /= chargeStepAmount;

        attackChargeTimer = attackChargeTimerStart;
        chargeStep = 0;
        //Debug.Log("ChargeAttack");
    }
    //Charge attack; charge attack state
    public void ChargeAttack()
    {
        attackChargeTimer -= Time.fixedDeltaTime;
        if (attackChargeTimer > 3f)
        {
            //Debug.Log(attackChargeTimer);
            chargeState = false;
        }
        else if (attackChargeTimer > 0f && chargeStep < chargeStepAmount)
        {
            AudioUtil.PlaySoundEffect(bossChargeSound,audioSource); 
            chargeStep++;
            chargeState = true;
            characterController.Move(chargeRichtung);
            //  Correct position of balls
            foreach (var ball in balls)
            {
                ball.transform.Translate(-chargeRichtung);
            }
        }
        else
        {
            if (attackChargeTimer >= 0)
                attackChargeTimer = 0;
            chargeState = false;
            chargeRichtung = Vector3.zero;
        }

    }

    //Charge attack; collision with player and damage
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = playerPlayer.gameObject.GetEntityId() == other.gameObject.GetEntityId();
        if (!isPlayer)
            return;

        playerMovement.PlayerMovementController.Move(chargeRichtung);
        if (chargeState == true)
        {
            playerPlayer.Damage(chargeDamage);
            chargeState = false;
            attackChargeTimer = 0f;
        }

    }


    public void AcidAttackActivate()
    {
        //Debug.Log("AcidAttack");
        Vector3 playerPosition = playerMovement.MoveTransform.position;

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

    public void DoGroundDamage(GameObject other)
    {
        if (attackGroundDidDamage)
            return;
        if (!other.TryGetComponent(out Player player))
            return;

        if (player.GetEntityId() == playerPlayer.GetEntityId())
        {
            attackGroundDidDamage = true;
            playerPlayer.Damage(groundDamage);
        }
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
        AudioUtil.PlaySoundEffect(bossDamageSound, audioSource);
        healthBar.ReduceHealth(amount);
    }
    public override void EntityHeal(float amount)
    {
        healthBar.IncreaseHealth(amount);
    }
}