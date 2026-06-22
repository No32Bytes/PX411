using UnityEngine;

public class TeacherDoorSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
    
        Debug.Log("Sensor hat etwas berührt: " + other.gameObject.name);

        DoorEntity door = other.GetComponent<DoorEntity>();
        if (door != null)
        {
            door.DoorOpen();
            Debug.Log("Penis");
        }
    }
}