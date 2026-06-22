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
    [SerializeField] private float chaseDistance = 25f;
    [SerializeField] private float startFightDistance = 100f;
    [SerializeField] private float rotationSpeed = 20f;
    [Header("Sound")]
    [SerializeField] private BaseSoundEffect damageSound;
    [SerializeField] private BaseSoundEffect startSound;
    [SerializeField] private BaseSoundEffect chaseSound;
    [SerializeField] private BaseSoundEffect chargeSound;
    [SerializeField] private BaseSoundEffect stompSound;
    [SerializeField] private BaseSoundEffect talkSound;
    [SerializeField] private BaseSoundEffect AcidThrowSound;
    [SerializeField] private SoundTrack bossMusic;
    [SerializeField] private float talkSoundDelay = 1f;
    [SerializeField] private GameObject bossModell;
    public Transform ballOfDoomSpawn;
    private Vector3 gravityVector;

    private PlayerMovement playerMovement;
    private Player playerPlayer;
    [Header("Attacks")]

    [SerializeField] private BallOfDoom[] balls;
    private readonly List<Vector3> throwDirections = new();
    [SerializeField] private float attackGroundTimerStart = 6f;
    [SerializeField] private float attackChargeTimerStart = 5f;
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
    private readonly List<AudioSource> audioSourceStack = new();
    [Header("AnimationParameters")]
    [SerializeField] private AnimationParamterInfo throwTrigger;
    [SerializeField] private AnimationParamterInfo stompTrigger;
    [SerializeField] private AnimationParamterInfo walkBool;

    enum SoundState
    {
        None,
        Chase,
        Charge,
        Stomp,
        Talk,
        AcidThrow,
    };
    bool fightStarted;
    bool fightStartedPlayed;
    SoundState soundState;
    float talkSoundLast;
    private string preBossSoundTrackGroup;
    private bool startMusic;
    private Camera playerCamera;
    private bool lookAtPlayer;

    private void Awake()
    {
        GlobalDataStore.GetStateManager().bossState.bossType = typeof(BossMuller).ToString();
        GlobalDataStore.GetStateManager().bossState.boss = gameObject;
        for (int i = 0; i < 3; i++)
        {
            AudioSource audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
            audioSource.spatialBlend = 0.5f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = startFightDistance * 2;
            audioSourceStack.Add(audioSource);
        }
        characterController = GetComponent<CharacterController>();
        fightStarted = false;
        fightStartedPlayed = false;
        startMusic = false;
        healthBar.SetOnDeathCallback(OnDeath);
        soundState = SoundState.None;
        lookAtPlayer = true;
    }

    private void Start()
    {
        playerCamera = GlobalDataStore.GetStateManager().playerState.playerRef.playerCamera;
        playerPlayer = GlobalDataStore.GetStateManager().playerState.player;
        playerMovement = playerPlayer.GetComponent<PlayerMovement>();
        overlayMullerUI.worldCamera = GlobalDataStore.GetStateManager().playerState.playerRef.playerOverlayCamera;

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


    }

    private void FixedUpdate()
    {
        bool startFight = false;
        if (!fightStarted)
        {
            if (GlobalDataStore.GetStateManager().playerState.playerRef == null)
                return;
            if (GlobalDataStore.GetStateManager().playerState.playerRef.playerCamera == null)
                return;
            Vector3 playerPos = GlobalDataStore.GetStateManager().playerState.playerRef.playerCamera.transform.position;
            float distance = (playerPos - transform.position).magnitude;
            if (distance > startFightDistance)
                return;
            startFight = true;
        }

        LookAtPlayer();
        //states for the attacks, every frame active, only activated by using the corresponding activate fuctions
        AttackGround();
        ChargeAttack();
        AcidAttack();

        //Timer for Attacks
        StartAttackTimer();

        if (!AudioSourceAllPlaying() && soundState == SoundState.None && Random.Range(1, 20) == 1 && talkSoundLast + talkSoundDelay < Time.fixedTime)
        {
            soundState = SoundState.Talk;
            talkSoundLast = Time.fixedTime;
        }

        if (soundState == SoundState.Talk)
        {
            Vector3 playerPos = GlobalDataStore.GetStateManager().playerState.playerRef.playerCamera.transform.position;
            float distance = (playerPos - transform.position).magnitude;
            if (distance >= chaseDistance)
                soundState = SoundState.Chase;
        }

        HandleSoundState();
        if (startFight)
            fightStarted = true;
    }

    private void HandleSoundState()
    {
        if (AudioSourceAllPlaying())
            return;

        BaseSoundEffect toPlay = null;
        switch (soundState)
        {
            case SoundState.Chase:
                toPlay = chaseSound;
                break;
            case SoundState.Charge:
                toPlay = chargeSound;
                break;
            case SoundState.Stomp:
                toPlay = stompSound;
                break;
            case SoundState.Talk:
                toPlay = talkSound;
                break;
            case SoundState.None:
            default:
                break;
        }

        if (startMusic && AudioSourceNumPlaying() != 0)
            return;

        if (startMusic)
        {
            GlobalDataStore.GetAudioManager().PlaySoundTrackGroup(bossMusic);
            startMusic = false;
        }
        if (!fightStartedPlayed)
        {
            toPlay = startSound;
            fightStartedPlayed = true;
            preBossSoundTrackGroup = GlobalDataStore.GetAudioManager().CurrentSoundTrackGroup;
            GlobalDataStore.GetAudioManager().Pause();
            startMusic = true;
            talkSoundLast = Time.fixedTime;
        }

        soundState = SoundState.None;
        AudioUtil.PlaySoundEffect(toPlay, AudioSourceGetFree());
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
    private int AudioSourceNumPlaying()
    {
        int playing = 0;
        foreach (var audioSource in audioSourceStack)
        {
            if (audioSource.isPlaying)
                playing++;
        }
        return playing;
    }
    private AudioSource AudioSourceGetFree()
    {
        foreach (AudioSource audioSource in audioSourceStack)
        {
            if (!audioSource.isPlaying)
                return audioSource;
        }
        return null;
    }
    private bool AudioSourceAllPlaying()
    {
        foreach (AudioSource audioSource in audioSourceStack)
        {
            if (!audioSource.isPlaying)
                return false;
        }
        return true;
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
        Debug.Log("GroundAttack");
        attackGroundDidDamage = false;
        attackGroundTimer = attackGroundTimerStart;
    }
    //ground attack; ground attack state
    public void AttackGround()
    {
        if (attackGroundTimer == attackGroundTimerStart)
        {
            soundState = SoundState.Stomp;
            stompTrigger.SetTrigger();
        }

        attackGroundTimer -= Time.fixedDeltaTime;
    }

    public void AttackGroundAnimation()
    {
        attackGround.Play();
        AudioUtil.PlaySoundEffect(stompSound, AudioSourceGetFree());
        stompTrigger.ResetTrigger();
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
        Debug.Log("ChargeAttack");
    }
    //Charge attack; charge attack state
    public void ChargeAttack()
    {
        if (soundState != SoundState.None)
            return;

        attackChargeTimer -= Time.fixedDeltaTime;
        if (attackChargeTimer > 3f)
        {
            Debug.Log(attackChargeTimer);
            lookAtPlayer = false;
            walkBool.ValueBool = true;
            chargeState = false;
        }
        else if (attackChargeTimer > 0f && chargeStep < chargeStepAmount)
        {
            if (chargeStep == 0)
                soundState = SoundState.Charge;
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
            walkBool.ValueBool = false;
            chargeState = false;
            lookAtPlayer = true;
            chargeRichtung = Vector3.zero;
        }

    }

    public void LookAtPlayer()
    {
        if (!lookAtPlayer)
            return;

        Vector3 dir = (playerCamera.transform.position - bossModell.transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            bossModell.transform.rotation = Quaternion.Slerp(bossModell.transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
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
        throwTrigger.SetTrigger();
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
            player.Damage(groundDamage);
        }

    }

    //Acid attack;  the acid attack state
    public void AcidAttackAnimation()
    {
        Vector3 playerPosition = playerCamera.transform.position;

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
        {
            if (ball.InUse)
                continue;
            if (ball.CanBallDamage())
                continue;

            if (throwDirections.Count == 0)
                break;

            ball.throwDirection = throwDirections[0];
            ball.ActiveTimer();
            throwDirections.RemoveAt(0);
        }


        AudioUtil.PlaySoundEffect(AcidThrowSound, AudioSourceGetFree());
        throwTrigger.ResetTrigger();

    }
    public void AcidAttack()
    {
        foreach (BallOfDoom ball in balls)
        {
            if (!ball.InUse || !ball.CanBallDamage())
                return;
            ball.transform.Translate(throwSpeed * Time.deltaTime * ball.throwDirection, Space.World);
        }
    }



    bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public Transform MullerTransform => characterController.transform;


    private void OnDeath()
    {
        GlobalDataStore.GetAudioManager().PlaySoundTrackGroupString(preBossSoundTrackGroup);
        Destroy(gameObject);
    }

    public override void EntityDamage(float amount)
    {
        AudioUtil.PlaySoundEffect(damageSound, AudioSourceGetFree());
        healthBar.ReduceHealth(amount);
    }
    public override void EntityHeal(float amount)
    {
        healthBar.IncreaseHealth(amount);
    }
}