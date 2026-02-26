using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EntityDraggable : MonoBehaviour
{
    const float MaxDistance = 5f;
    const float MaxLinearVelocity = 10f;
    public static EntityDraggable CurrentDraggedEntity { get; private set; } = null;
    public static bool IsEntitySelected { get; private set; } = false;
    public static Camera playerCamera = null;

    private bool isSelected = false;
    private float distance = 0f;
    private Vector3 remainingMoveVector = Vector3.zero;
    private float linearDampingSave;
    private Rigidbody entityRigibody;
    private void Awake()
    {
        entityRigibody = GetComponent<Rigidbody>();
        entityRigibody.maxLinearVelocity = MaxLinearVelocity;
        isSelected = false;
    }
    private void FixedUpdate()
    {
        if (!isSelected || !playerCamera)
            return;

        Vector3 startPosition = gameObject.transform.position;
        float newDistance = (startPosition - playerCamera.transform.position).magnitude;

        if(distance == 0)
            distance = newDistance;
        if(newDistance >= MaxDistance)
        {
            DeselectEntity();
            return;
        }
        Vector3 endPosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 moveVector = endPosition - startPosition;

        Move(moveVector + remainingMoveVector);
    }
    private void Move(Vector3 moveVector)
    {
        Vector3 partialMoveVector = CalculatePartialMoveVector(moveVector);
        entityRigibody.AddForce(partialMoveVector * partialMoveVector.magnitude,ForceMode.Impulse);
        entityRigibody.linearDamping = 1 / partialMoveVector.sqrMagnitude;
        remainingMoveVector = moveVector - partialMoveVector;
    }
    private Vector3 CalculatePartialMoveVector(Vector3 moveVector)
    {
        const float ignoreCalcDistance = 0.1f;

        const float closeDistanceMax = 0.4f;
        const float closeDistanceMult = -0.05f;
        const float closeDistanceBegin = 8f;

        const float longDistanceMult = -0.01f;
        const float longDistanceMax = 0.4f;

        float moveDistance = moveVector.magnitude;
        float partialMoveVectorMultSubtract;
        if(moveDistance <= ignoreCalcDistance)
            partialMoveVectorMultSubtract = 0f;
        else if(moveDistance <= closeDistanceBegin)
            partialMoveVectorMultSubtract = closeDistanceMax * Mathf.Exp(closeDistanceMult * moveDistance);
        else 
            partialMoveVectorMultSubtract = longDistanceMax * Mathf.Exp(longDistanceMult * (moveDistance - closeDistanceBegin));
        
        return moveVector * (1 - partialMoveVectorMultSubtract);
    }
    public void SelectEntity(Camera playerCamera)
    {
        isSelected = true;
        IsEntitySelected = isSelected;
        entityRigibody.useGravity = true;

        CurrentDraggedEntity = this;
        EntityDraggable.playerCamera = playerCamera;
        distance = 0f;
        remainingMoveVector = Vector3.zero;
        linearDampingSave = entityRigibody.linearDamping;
    }
    public void DeselectEntity()
    {
        isSelected = false;
        IsEntitySelected = isSelected;
        entityRigibody.useGravity = true;
        entityRigibody.linearDamping = linearDampingSave;
    }
}