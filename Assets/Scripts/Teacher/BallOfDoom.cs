using UnityEngine;


public class BallOfDoom : MonoBehaviour
{

    private Player playerPlayer;
    [SerializeField] private BossMuller muller;

    private bool canDamage = false;

    private float timer;
    [SerializeField] private float timerMax = 2f;
    [SerializeField] private int damage = 10;

    private bool timerActive = false;

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
        canDamage = true;
        timer = timerMax;
        timerActive = true;
    }


}
