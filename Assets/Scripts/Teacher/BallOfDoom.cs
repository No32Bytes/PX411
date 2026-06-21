using System.Runtime.CompilerServices;
using UnityEngine;


public class BallOfDoom : MonoBehaviour
{

    private Player playerPlayer;
    private BossMuller muller;
    private bool canDamage = false;
    private bool inUse = false;
    public bool InUse => inUse;
    [HideInInspector] public Vector3 throwDirection;
    private Vector3 rotationDirection;
    private float timer;
    [SerializeField] private float timerMax = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private BaseSoundEffect collisionSoundEffect;

    private bool timerActive = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        if (typeof(BossMuller).ToString() != GlobalDataStore.GetStateManager().bossState.bossType)
        {
            Debug.Log("BallOfDoom must be in scence with the given boss");
            return;
        }
        muller = GlobalDataStore.GetStateManager().bossState.boss.GetComponent<BossMuller>();
        playerPlayer = GlobalDataStore.GetStateManager().playerState.player;
        BackToMuller();
    }

    void Update()
    {

        if (timerActive == true)
        {
            TimerUpdate();
        }
        if (inUse && !CanBallDamage())
            BackToMuller();

        gameObject.transform.Rotate(rotationSpeed * Time.deltaTime * rotationDirection, Space.World);
    }



    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = playerPlayer.gameObject.GetEntityId() == other.gameObject.GetEntityId();
        if (isPlayer && canDamage)
        {
            AudioUtil.PlaySoundEffect(collisionSoundEffect, audioSource);
            playerPlayer.Damage(damage, false);
            canDamage = false;
            BackToMuller();
        }
    }

    public bool CanBallDamage()
    {
        return canDamage;
    }

    public void BackToMuller()
    {
        transform.position = muller.MullerTransform.position;
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    private void TimerUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timerActive = false;
            canDamage = false;
            inUse = false;
            BackToMuller();
            timer = timerMax;

        }
    }

    public void ActiveTimer()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
        canDamage = true;
        timer = timerMax;
        timerActive = true;
        inUse = true;
        rotationDirection = new Vector3(Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10)).normalized;
    }


}
