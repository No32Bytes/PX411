
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

        float min = Mathf.Min(targetRotationClose, targetRotationOpen);
        float max = Mathf.Max(targetRotationClose, targetRotationOpen);

        float clamp = Mathf.Clamp(GetCurrentRotation(), min, max);
        if (clamp == GetCurrentRotation() && antiClockWise && !isOpen)
            SetRotation(targetRotation);


        if (clamp != GetCurrentRotation() && !antiClockWise && !isOpen)
            SetRotation(targetRotation);


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