using UnityEngine;

public class SafeButtonEntityBridge : BaseEntity
{
    private SafeButton targetButton;

    private void Start()
    {
        targetButton = GetComponent<SafeButton>();
        if (targetButton == null)
        {

        }
    }

    public override void EntityInteraction()
    {
        if (targetButton != null)
        {
            targetButton.TriggerButton();
        }
    }
}