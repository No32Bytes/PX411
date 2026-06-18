using TMPro;
using UnityEngine;

class DoorEntityInformation : BaseEntityInformation
{
    [SerializeField] private TMP_Text actionText;
    private DoorEntity doorEntity;
    private bool internalOpen;
    private void Awake()
    {
        if (!SourceGameObject.TryGetComponent(out doorEntity))
            return;

        SetActionText(doorEntity.IsOpen);
    }

    private void FixedUpdate()
    {
        if (doorEntity.IsOpen != internalOpen)
            SetActionText(doorEntity.IsOpen);
    }

    private void SetActionText(bool isOpen)
    {
        if (isOpen)
            actionText.text = "Close Door";
        else
            actionText.text = "Open Door";

        internalOpen = isOpen;
    }
}