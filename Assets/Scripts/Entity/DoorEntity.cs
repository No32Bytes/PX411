using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DoorEntity : BaseEntity
{
    private bool isOpen = false;
    public override void EntityInteraction()
    {

        isOpen = !isOpen;
    }
}