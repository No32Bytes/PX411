
using System;
using UnityEngine;

public class DoorEntity : BaseEntity
{
    [SerializeField] private float targetRotationClose;
    [SerializeField] private float targetRotationOpen;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool antiClockWise = false;
    [SerializeField] private bool isOpen = false;
    public bool IsOpen => isOpen;
    [SerializeField] private BaseSoundEffect doorMovementSound;
    private AudioSource audioSource;
    private float targetRotation;
    private readonly float clipDoorAngle = 0.0005f;
    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        entityId = "door";
        usePhysics = false;

        if (targetRotationClose > targetRotationOpen)
            Debug.LogError("Error: targetRotationClose must be smaller than targetRotationOpen + " + transform.position + transform.name);
    }

    private void Start()
    {
        float startRotation = isOpen ? targetRotationOpen : targetRotationClose;
        targetRotation = startRotation;
        SetRotation(startRotation);
    }
    private void FixedUpdate()
    {
        UpdateDoorInfoView();
    }
    private void Update()
    {
        float currenRotation = GetCurrentRotation();
        float absoluteDiff = Math.Abs(currenRotation) - Math.Abs(targetRotation);
        if (currenRotation == targetRotation || Math.Abs(absoluteDiff) < clipDoorAngle)
        {
            audioSource.Stop();
            return;
        }

        if (!audioSource.isPlaying)
            AudioUtil.PlaySoundEffect(doorMovementSound, audioSource);

        bool getDifferentAngle = false;
        if (!isOpen)
            getDifferentAngle = true;
        if (antiClockWise)
            getDifferentAngle = !getDifferentAngle;

        float rotationChangeMax = targetRotation - currenRotation;
        if (getDifferentAngle)
        {
            if (rotationChangeMax > 0)
                rotationChangeMax = 360 - rotationChangeMax;
            else
                rotationChangeMax = 360 + rotationChangeMax;
        }

        if (!isOpen)
            rotationChangeMax *= -1;
        if (antiClockWise)
            rotationChangeMax *= -1;

        float rotationChange = rotationChangeMax * rotationSpeed * Time.deltaTime;

        if (Math.Abs(rotationChange) > Math.Abs(rotationChangeMax))
            rotationChange = rotationChangeMax;

        if (Math.Abs(rotationChange) < clipDoorAngle)
        {
            SetRotation(targetRotation);
            return;
        }


        transform.Rotate(0, rotationChange, 0, Space.World);

        float minValid = targetRotationOpen;
        float maxValid = targetRotationClose;
        if (minValid > maxValid)
            (minValid, maxValid) = (maxValid, minValid);

        if (!antiClockWise)
        {
            if (GetCurrentRotation() > maxValid || GetCurrentRotation() < minValid)
                SetRotation(targetRotation);
        }

        if (antiClockWise)
        {
            minValid = 360 - minValid;
            if (GetCurrentRotation() > minValid || GetCurrentRotation() < maxValid)
                SetRotation(targetRotation);
        }

    }
    private float GetCurrentRotation()
    {
        float currenRotation = transform.eulerAngles.y;
        if (currenRotation < 0)
            currenRotation = 360 - currenRotation;
        return currenRotation;
    }

    private void SetRotation(float rotation)
    {
        Vector3 vec = transform.rotation.eulerAngles;
        vec.y = rotation;
        transform.eulerAngles = vec;
    }

    public override void EntityInteraction()
    {
        ToggleDoorState();
    }
    protected void ToggleDoorState()
    {
        audioSource.Stop();
        if (isOpen)
            DoorClose();
        else
            DoorOpen();
    }
    public void DoorOpen()
    {
        targetRotation = targetRotationOpen;
        isOpen = true;

        UpdateDoorInfoView();
    }

    public void DoorClose()
    {

        targetRotation = targetRotationClose;
        if (targetRotation == 0)
            targetRotation = 360;

        isOpen = false;
        UpdateDoorInfoView();
    }

    public void UpdateDoorInfoView()
    {
        if (isOpen)
            EntityInformationView.SetInteractInfo(gameObject, "Schließen");
        else
            EntityInformationView.SetInteractInfo(gameObject, "Öffnen");
    }
}