using Entity;
using InputUtil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TeacherMovement : MonoBehaviour
{
    public Transform[] targets;
    public float speed = 5f;

    private int currentTargetIndex = 0;

    void Run()
    {
        if (targets.Length == 0) return;

        Transform target = targets[currentTargetIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentTargetIndex++;

            if (currentTargetIndex >= targets.Length)
            {
                currentTargetIndex = 0;
            }
        }
    }
    private void Start()
     {
       DebugDev.DebugFunction.RegisterDebugCallback(Run);
    }
    
}