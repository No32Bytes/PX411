using System;
using UnityEngine;

public class EntityDraggable : MonoBehaviour
{
    public static EntityDraggable CurrentDraggedEntity { get; private set; } = null;
    public static bool IsEntitySelected { get; private set; } = false;
    public static Camera playerCamera = null;

    private bool isSelected = false;
    private float distance = 0f;
    private Vector3 remainingMoveVector;
    private void Awake()
    {
        isSelected = false;
    }
    private void Update()
    {
        if (!isSelected || !playerCamera)
            return;

        if (distance == 0)
            UpdateDistance();

        Vector3 startPosition = gameObject.transform.position;
        Vector3 endPosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 moveVector = endPosition - startPosition;


        Vector3 totalMoveVector = moveVector + remainingMoveVector;
        Vector3 partialMoveVector = CalculatePartialMoveVector(totalMoveVector);

        transform.position += partialMoveVector;
        remainingMoveVector = totalMoveVector - partialMoveVector;
    }
    public void UpdateDistance()
    {
        Vector3 startPosition = gameObject.transform.position;
        distance = (startPosition - playerCamera.transform.position).magnitude;
    }
    private Vector3 CalculatePartialMoveVector(Vector3 totalMoveVector)
    {
        const float ignoreCalcDistance = 0.1f;

        const float closeDistanceMax = 0.4f;
        const float closeDistanceMult = -0.05f;
        const float closeDistanceBegin = 8f;

        const float longDistanceMult = -0.01f;
        const float longDistanceMax = 0.4f;

        float moveDistance = totalMoveVector.magnitude;
        float partialMoveVectorMultSubtract;
        if(moveDistance <= ignoreCalcDistance)
            partialMoveVectorMultSubtract = 0f;
        else if(moveDistance <= closeDistanceBegin)
            partialMoveVectorMultSubtract = closeDistanceMax * Mathf.Exp(closeDistanceMult * moveDistance);
        else 
            partialMoveVectorMultSubtract = longDistanceMax * Mathf.Exp(longDistanceMult * (moveDistance - closeDistanceBegin));
        
        return totalMoveVector * (1 - partialMoveVectorMultSubtract);
    }
    public void SelectEntity(Camera playerCamera)
    {
        isSelected = true;
        IsEntitySelected = isSelected;

        CurrentDraggedEntity = this;
        EntityDraggable.playerCamera = playerCamera;
        distance = 0f;
        remainingMoveVector = Vector3.zero;
    }
    public void DeselectEntity()
    {
        isSelected = false;
        IsEntitySelected = isSelected;
    }
}