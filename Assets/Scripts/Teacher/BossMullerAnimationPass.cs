using UnityEngine;

public class BossMullerAnimationPass : MonoBehaviour
{
    [SerializeField] private BossMuller bossMuller;
    public void AcidAnimationPass()
    {
        bossMuller.AcidAttackAnimation();
    }

    public void StompAnimationPass()
    {
        bossMuller.AttackGroundAnimation();
    }
}