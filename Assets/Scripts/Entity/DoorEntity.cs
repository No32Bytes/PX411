using System;
using System.IO.Compression;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DoorEntity : BaseEntity
{
    [SerializeField] private float targetRotationClose;
    [SerializeField] private float targetRotationOpen;
    [SerializeField] private float maxRotationChange = 180f;
    [SerializeField] private bool rotateAroundZAxis = false;
    [SerializeField] private bool antiClockWise = false;
    [SerializeField] private bool useRotationZAxis = false;
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
    protected void Start()
    {
        float startRotation = isOpen ? targetRotationOpen : targetRotationClose;

        Vector3 angles = new(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        if (useRotationZAxis)
            angles.z = startRotation;
        else
            angles.y = startRotation;

        transform.localEulerAngles = angles;
        targetRotation = GetCurrentRotation();
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
    private void Update()
    {
        float currenRotation = GetCurrentRotation();
        if (currenRotation < 0)
            currenRotation = 360 - currenRotation;
        if (currenRotation == targetRotation)
        {
            audioSource.Stop();
            return;
        }
        Debug.Log(currenRotation);

        if (!audioSource.isPlaying)
        {
            AudioUtil.PlaySoundEffect(doorMovementSound, audioSource);
        }

        float rotationChange = targetRotation - currenRotation;
        if (isOpen && rotationChange < 0)
            rotationChange *= -1;
        if (!isOpen && rotationChange > 0)
            rotationChange *= -1;


        if (antiClockWise)
            rotationChange *= -1;




        if (Math.Abs(rotationChange) > maxRotationChange)
            rotationChange = Mathf.Clamp(rotationChange, -maxRotationChange, maxRotationChange);

        if (Math.Abs(rotationChange) < 1)
        {
            Vector3 setNextRotation = new(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (useRotationZAxis)
                setNextRotation.z = targetRotation;
            else
                setNextRotation.y = targetRotation;

            transform.localEulerAngles = setNextRotation;
            return;
        }

        rotationChange *= Time.deltaTime;
        Vector3 nextRotation = new(0, 0, 0);
        SetRotationVector(ref nextRotation, rotationChange);
        transform.Rotate(nextRotation);
    }
    private void SetRotationVector(ref Vector3 rotation, float value)
    {
        if (rotateAroundZAxis)
        {
            rotation.z = value;
            return;
        }
        rotation.y = value;
    }
    private float GetCurrentRotation()
    {
        if (useRotationZAxis)
            return transform.localEulerAngles.z;

        return transform.localEulerAngles.y;
    }
}