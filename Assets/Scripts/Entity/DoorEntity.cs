using System;
using UnityEngine;

public class DoorEntity : BaseEntity
{
    [SerializeField] private float targetRotationClose;
    [SerializeField] private float targetRotationOpen;
    [SerializeField] private float rotationSpeed = 100f; // Tipp: Höherer Wert, da Grad pro Sekunde gesucht sind
    [SerializeField] private bool isOpen = false;
    public bool IsOpen => isOpen;
    [SerializeField] private BaseSoundEffect doorMovementSound;
    private AudioSource audioSource;
    private float targetRotation;
    private readonly float clipDoorAngle = 0.05f;

    protected void Awake()
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
        float currentRotation = GetCurrentRotation();

        // Nutzt Unitys eingebaute Kreis-Differenz-Berechnung
        if (Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation)) < clipDoorAngle)
        {
            SetRotation(targetRotation);
            audioSource.Stop();
            return;
        }

        if (!audioSource.isPlaying)
            AudioUtil.PlaySoundEffect(doorMovementSound, audioSource);

        // Berechnet die neue Rotation sicher über den 360°-Kreis hinweg
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        SetRotation(newRotation);
    }

    private float GetCurrentRotation()
    {
        float currentRotation = transform.eulerAngles.y;
        if (currentRotation < 0)
            currentRotation = 360f + currentRotation;
        return currentRotation;
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