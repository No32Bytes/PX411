using UnityEngine;

public class DamagePlayer : MonoBehaviour
{

    [SerializeField] private Player playerPlayer;
    [SerializeField] private TeacherMullerEntity muller;

    private bool canDamage = false;

    private float timer;
    [SerializeField] private float timerMax = 2f;
    [SerializeField] private int damage = 10;

    private bool timerActive = false;

    

    void Update()
    {
        
        if (timerActive == true)
        {
            timerState();
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && canDamage)
        {
            playerPlayer.Damage(damage);
            canDamage = false;
            backToMuller();
        }
    }

    public bool CanBallDamage()
    {
        return canDamage;
    }

    public void backToMuller()
    {
        transform.position = muller.mullerCoordinations().position;
    }

    private void timerState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timerActive = false;
            canDamage = false;
            backToMuller();
            timer = timerMax;
            
        }
    }

    public void activateTimer()
    {
        canDamage = true;
        timer = timerMax;
        timerActive = true;
    }


}
