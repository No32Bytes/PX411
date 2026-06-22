using UnityEngine;

public class TeacherDoorSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
    
        //Debug.Log("Sensor hat etwas berührt: " + other.gameObject.name);
        if (other.TryGetComponent<DoorEntity>(out var door))
        {
            door.DoorOpen();
            //Debug.Log("Penis");
        }
    }
}