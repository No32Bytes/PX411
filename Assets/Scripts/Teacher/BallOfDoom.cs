using UnityEngine;


public class BallOfDoom : MonoBehaviour
{

    private Player playerPlayer;
    [SerializeField] private BossMuller muller;
    private bool canDamage = false;

    private float timer;
    [SerializeField] private float timerMax = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private BaseSoundEffect collisionSoundEffect;

    private bool timerActive = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
    }

    void Start()
    {
        playerPlayer = GlobalDataStore.GetStateManager().playerState.player;
    }

    void Update()
    {

        if (timerActive == true)
        {
            TimerUpdate();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = playerPlayer.gameObject.GetEntityId() == other.gameObject.GetEntityId();
        if (isPlayer && canDamage)
        {
            AudioUtil.PlaySoundEffect(collisionSoundEffect, audioSource);
            playerPlayer.Damage(damage);
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
        gameObject.SetActive(false);
    }

    private void TimerUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timerActive = false;
            canDamage = false;
            BackToMuller();
            timer = timerMax;

        }
    }

    public void ActiveTimer()
    {
        gameObject.SetActive(true);
        canDamage = true;
        timer = timerMax;
        timerActive = true;
    }


}
