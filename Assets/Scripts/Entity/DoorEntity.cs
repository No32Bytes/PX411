using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DoorEntity : BaseEntity
{
    [SerializeField] private float targetRotationClose;
    [SerializeField] private float targetRotationOpen;
    [SerializeField] private float maxRotationChange = 180f;
    [SerializeField] private bool isOpen = false;
    private float targetRotation;
    protected override void EntityAwake()
    {
        float startRotation = isOpen ? targetRotationOpen : targetRotationClose;
        transform.rotation = Quaternion.Euler(transform.rotation.x, startRotation, transform.rotation.z);
    }
    public override void EntityInteraction()
    {
        UpdateDoorState();
    }
    protected void UpdateDoorState()
    {
        if (isOpen)
            DoorClose();
        else
            DoorOpen();
    }
    private void Update()
    {
        float currenRotation = transform.rotation.eulerAngles.y;
        if (currenRotation == targetRotation)
            return;

        float rotationChange = targetRotation - currenRotation;
        if (Math.Abs(rotationChange) > maxRotationChange)
            rotationChange = Mathf.Clamp(rotationChange, -maxRotationChange, maxRotationChange);

        if (Math.Abs(rotationChange) < 1)
        {
            Vector3 setNextRotation = new(transform.rotation.x, targetRotation, transform.rotation.z);
            transform.rotation = Quaternion.Euler(setNextRotation);
            return;
        }

        rotationChange *= Time.deltaTime;
        Vector3 nextRotation = new(0, rotationChange, 0);
        transform.Rotate(nextRotation);

    }
    private void DoorOpen()
    {
        targetRotation = targetRotationOpen;
        isOpen = true;
    }

    private void DoorClose()
    {
        targetRotation = targetRotationClose;
        isOpen = false;
    }
}