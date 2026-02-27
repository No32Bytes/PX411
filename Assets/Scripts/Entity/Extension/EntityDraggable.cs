using System;
using UnityEngine;

public struct EntityDraggableConfig
{
    public const float MaxDistance = 5f;
    public const float MaxLinearVelocity = 10f;
    public const float selectedMultMass = 10f;
    public const float IgnoreCalcDistance = 0.1f;
    public const float CloseDistanceBegin = 8f;

    public const float CloseDistanceMax = 0.4f;
    public const float CloseDistanceMult = -0.05f;

    public const float LongDistanceMax = 0.4f;
    public const float LongDistanceMult = -0.01f;


    public const float antiGravityModifier = 9;
    public const float targetPositionSlowDown = 2;
}


[RequireComponent(typeof(Rigidbody))]
public class EntityDraggable : MonoBehaviour
{
    public static EntityDraggable CurrentDraggedEntity { get; private set; } = null;
    public static bool IsEntitySelected()
    {
        if (CurrentDraggedEntity == null)
            return false;
        return CurrentDraggedEntity.isSelected;
    }
    public static Camera playerCamera = null;

    private bool isSelected = false;
    private float distance = 0f;
    private Vector3 remainingMoveVector = Vector3.zero;
    private float linearDampingSave;
    private Rigidbody entityRigibody;
    private void Awake()
    {
        entityRigibody = GetComponent<Rigidbody>();
        entityRigibody.maxLinearVelocity = EntityDraggableConfig.MaxLinearVelocity;
        isSelected = false;
    }
    private void FixedUpdate()
    {
        if (!isSelected || !playerCamera)
            return;

        Vector3 startPosition = gameObject.transform.position;
        float newDistance = (startPosition - playerCamera.transform.position).magnitude;

        if (distance == 0)
            distance = newDistance;
        if (newDistance >= EntityDraggableConfig.MaxDistance)
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
        entityRigibody.AddForce(partialMoveVector * partialMoveVector.magnitude, ForceMode.Impulse);
        entityRigibody.AddForce(EntityDraggableConfig.antiGravityModifier * entityRigibody.mass * Vector3.up);
        entityRigibody.linearDamping = EntityDraggableConfig.targetPositionSlowDown / moveVector.sqrMagnitude;

        remainingMoveVector = moveVector - partialMoveVector;
    }
    private Vector3 CalculatePartialMoveVector(Vector3 moveVector)
    {
        float moveDistance = moveVector.magnitude;
        float partialMoveVectorMultSubtract;
        if (moveDistance <= EntityDraggableConfig.IgnoreCalcDistance)
            partialMoveVectorMultSubtract = 0f;
        else if (moveDistance <= EntityDraggableConfig.CloseDistanceBegin)
            partialMoveVectorMultSubtract = EntityDraggableConfig.CloseDistanceMax * Mathf.Exp(EntityDraggableConfig.CloseDistanceMult * moveDistance);
        else
            partialMoveVectorMultSubtract = EntityDraggableConfig.LongDistanceMax * Mathf.Exp(EntityDraggableConfig.LongDistanceMult * (moveDistance - EntityDraggableConfig.CloseDistanceBegin));

        return moveVector * (1 - partialMoveVectorMultSubtract);
    }
    public void SelectEntity(Camera playerCamera)
    {
        isSelected = true;
        CurrentDraggedEntity = this;

        EntityDraggable.playerCamera = playerCamera;
        distance = 0f;
        remainingMoveVector = Vector3.zero;
        linearDampingSave = entityRigibody.linearDamping;
        entityRigibody.mass *= EntityDraggableConfig.selectedMultMass;
    }
    public void DeselectEntity()
    {
        isSelected = false;

        entityRigibody.linearDamping = linearDampingSave;
        entityRigibody.mass /= EntityDraggableConfig.selectedMultMass;
    }
}