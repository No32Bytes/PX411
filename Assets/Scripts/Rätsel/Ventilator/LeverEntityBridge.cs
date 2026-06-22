using UnityEngine;

public class LeverEntityBridge : BaseEntity
{
    private LeverButton targetLever;

    private void Start()
    {
        targetLever = GetComponent<LeverButton>();
    }

    public override void EntityInteraction()
    {
        if (targetLever != null)
        {
            targetLever.ToggleLever();
        }
    }
}