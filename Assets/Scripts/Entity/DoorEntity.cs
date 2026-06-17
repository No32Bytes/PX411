
using System;
using UnityEngine;

public class DoorEntity : BaseEntity
{
    [SerializeField] private float targetRotationClose;
    [SerializeField] private float targetRotationOpen;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private bool antiClockWise = false;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private BaseSoundEffect doorMovementSound;
    private AudioSource audioSource;
    private float targetRotation;
    private void Awake()
    {
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        entityId = "door";
        usePhysics = false;
    }

    private void Start()
    {
        float startRotation = isOpen ? targetRotationOpen : targetRotationClose;
        targetRotation = startRotation;
        SetRotation(startRotation);
    }

    private void Update()
    {
        float currenRotation = transform.eulerAngles.y;
        if (currenRotation < 0)
            currenRotation = 360 - currenRotation;

        if (currenRotation == targetRotation)
        {
            audioSource.Stop();
            return;
        }

        if (!audioSource.isPlaying)
            AudioUtil.PlaySoundEffect(doorMovementSound, audioSource);

        float rotationChangeMax = targetRotation - currenRotation;
        if (isOpen && rotationChangeMax < 0)
            rotationChangeMax *= -1;
        if (!isOpen && rotationChangeMax > 0)
            rotationChangeMax *= -1;

        if(antiClockWise)
            rotationChangeMax *= -1;

        float rotationChange = rotationChangeMax * rotationSpeed * Time.deltaTime;

        if (Math.Abs(rotationChange) > Math.Abs(rotationChangeMax))
            rotationChange = rotationChangeMax;

        SetRotation(currenRotation + rotationChange);
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
    }

    public void DoorClose()
    {
        targetRotation = targetRotationClose;
        isOpen = false;
    }
}