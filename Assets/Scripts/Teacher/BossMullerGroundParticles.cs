using UnityEngine;

public class BossMullerGroundParticles : MonoBehaviour
{
    [SerializeField] private BossMuller bossMuller;
    private void OnParticleCollision(GameObject other)
    {
        bossMuller.DoGroundDamage(other);
    }

}
