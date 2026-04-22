using Entity;
using InputUtil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TeacherMovement : MonoBehaviour
{
    public Transform[] targets;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    public Transform playerCamera;     
    public float stopDistance = 5f;   

    private int currentTargetIndex = 0;
    private bool isStopped = false;

    void Run()
    {
        if (targets.Length == 0) return;

        
        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        if (distanceToPlayer < stopDistance)
        {
            isStopped = true;
        }
        else
        {
            isStopped = false;
        }

        if (isStopped)
        {
            
            Vector3 dir = (playerCamera.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Transform target = targets[currentTargetIndex];

         
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

       
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                currentTargetIndex++;

                
                if (currentTargetIndex >= targets.Length)
                {
                    currentTargetIndex = 0;
                }
            }
        }
    }
    private void Start()
     {
       DebugDev.DebugFunction.RegisterDebugCallback(Run);
    }
    
}