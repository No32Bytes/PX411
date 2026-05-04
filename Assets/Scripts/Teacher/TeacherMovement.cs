using Entity;
using InputUtil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AI;

public class TeacherMovement : MonoBehaviour
{
    public Transform[] targets;
    public Transform playerCamera;

    public float speed = 2f;
    static float chaseSpeed = 2f;
    public float rotationSpeed = 5f;

    public float stopDistance = 5f;
    public float loseDistance = 15f;

    public GameObject rightArmContainer;

    public float timeUntilChase = 3f;

    private int currentTargetIndex = 0;

    private float timer = 0f;

    private bool isChasing = false;
    private bool isReturning = false;

    private List<Vector3> chasePath = new List<Vector3>();
    private int returnIndex = 0;

    void Run()
    {
        float dist = Vector3.Distance(transform.position, playerCamera.position);
        bool holding = rightArmContainer.activeSelf;

        if (!isChasing && !isReturning)
        {
            if (dist < stopDistance && holding)
            {
                timer += Time.deltaTime;
                LookAtPlayer();

                if (timer >= timeUntilChase)
                {
                    isChasing = true;
                    chasePath.Clear();
                }

                return;
            }

            timer = 0f;
            Patrol();
            return;
        }

        if (isChasing)
        {
            chasePath.Add(transform.position);

            MoveTo(playerCamera.position, chaseSpeed);

            if (dist > loseDistance)
            {
                isChasing = false;
                isReturning = true;
                returnIndex = chasePath.Count - 1;
            }

            return;
        }

        if (isReturning)
        {
            if (returnIndex >= 0)
            {
                MoveTo(chasePath[returnIndex], speed);

                if (Vector3.Distance(transform.position, chasePath[returnIndex]) < 0.3f)
                {
                    returnIndex--;
                }
            }
            else
            {
                isReturning = false;
                chasePath.Clear();
            }

            return;
        }
    }

    void Patrol()
    {
        if (targets.Length == 0) return;

        Transform target = targets[currentTargetIndex];

        MoveTo(target.position, speed);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
        }
    }

    void MoveTo(Vector3 pos, float spd)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            pos,
            spd * Time.deltaTime
        );

        Vector3 dir = (pos - transform.position).normalized;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = (playerCamera.position - transform.position).normalized;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    private void Start()
     {
       DebugDev.DebugFunction.RegisterDebugCallback(Run);
    }
    
}