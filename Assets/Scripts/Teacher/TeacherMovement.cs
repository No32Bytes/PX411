using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class TeacherMovement : MonoBehaviour
{
    [Header("Zuweisungen")]
    public Transform[] targets;
    public Transform playerCamera;
    public GameObject rightArmContainer;
    public Transform eyePosition;

    [Header("Audio-Einstellungen")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private BaseSoundEffect footstepClip;
    [SerializeField] private float patrolStepInterval = 0.35f;
    [SerializeField] private float chaseStepInterval = 0.18f;

    [Header("Geschwindigkeiten")]
    public float speed = 2f;
    readonly float chaseSpeed = 12f;
    public float rotationSpeed = 5f;

    [Header("Distanzen & Timer")]
    public float stopDistance = 3f;
    public float chaseDistanceWithPhone = 8f;
    public float loseDistance = 15f;
    public float timeUntilChase = 3f;
    public float loseChaseCooldown = 2.5f;

    [Header("Schadens-Einstellungen")]
    [SerializeField] private float damageDistance = 2f;
    [SerializeField] private float damagePerSecond = 20f;

    private int currentTargetIndex = 0;
    private float timer = 0f;
    private float loseTimer = 0f;
    private float footstepTimer = 0f;
    private bool isChasing = false;
    private bool isReturning = false;
    private bool isCrossingLink = false;

    private Vector3 returnPoint;
    private Vector3 lastKnownPlayerPosition;

    private Player playerPlayer;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed * 50f; 
        agent.acceleration = 20f;
        agent.updateRotation = true; 

        var stateManager = GlobalDataStore.GetStateManager();
        if (stateManager != null && stateManager.playerState != null)
        {
            playerPlayer = stateManager.playerState.player;
        }

        if (eyePosition == null)
        {
            eyePosition = transform;
        }

        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (agent.isOnOffMeshLink && !isCrossingLink)
        {
            StartCoroutine(WalkThroughDoor());
            return;
        }

        if (isCrossingLink) return;

        HandleFootsteps();

        float dist = Vector3.Distance(transform.position, playerCamera.position);
        bool holding = rightArmContainer.activeSelf;
        float currentDetectionDistance = holding ? chaseDistanceWithPhone : stopDistance;

        bool playerIsVisible = HasLineOfSight(currentDetectionDistance);
        bool playerIsVisibleInChase = HasLineOfSight(loseDistance);

        if (isReturning && playerIsVisible)
        {
            isReturning = false;
            isChasing = true;
            loseTimer = 0f;
            timer = timeUntilChase; 
            returnPoint = transform.position;
        }

        if (!isChasing && !isReturning)
        {
            if (playerIsVisible)
            {
                agent.ResetPath();
                timer += Time.deltaTime;
                LookAtPlayer();

                if (timer >= timeUntilChase)
                {
                    isChasing = true;
                    returnPoint = transform.position;
                }
                return;
            }

            timer = 0f;
            Patrol();
            return;
        }

        if (isChasing)
        {
            agent.speed = chaseSpeed;

            if (playerIsVisibleInChase && dist <= loseDistance)
            {
                loseTimer = 0f;
                lastKnownPlayerPosition = playerCamera.position;
                agent.SetDestination(lastKnownPlayerPosition);
            }
            else
            {
                loseTimer += Time.deltaTime;
                agent.SetDestination(lastKnownPlayerPosition);
            }

            if (dist <= damageDistance && playerPlayer != null)
            {
                playerPlayer.Damage(damagePerSecond);
            }

            if (loseTimer >= loseChaseCooldown || dist > loseDistance * 1.5f)
            {
                isChasing = false;
                isReturning = true;
                agent.SetDestination(returnPoint);
            }
            return;
        }

        if (isReturning)
        {
            agent.speed = speed;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isReturning = false;
            }
            return;
        }
    }

    private void HandleFootsteps()
    {
        if (agent.remainingDistance > 0.1f && !agent.isStopped)
        {
            footstepTimer += Time.deltaTime;
            float currentInterval = isChasing ? chaseStepInterval : patrolStepInterval;

            if (footstepTimer >= currentInterval)
            {
                if (footstepAudioSource != null && footstepClip != null)
                {
                    AudioUtil.PlaySoundEffect(footstepClip, footstepAudioSource);
                }
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private bool HasLineOfSight(float maxDistance)
    {
        Vector3 directionToPlayer = playerCamera.position - eyePosition.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > maxDistance) return false;

        if (Physics.Raycast(eyePosition.position, directionToPlayer.normalized, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == playerCamera || hit.transform.root == playerCamera.root)
            {
                return true;
            }
        }

        return false;
    }

    void Patrol()
    {
        if (targets.Length == 0) return;

        agent.speed = speed;
        Transform target = targets[currentTargetIndex];
        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = (playerCamera.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator WalkThroughDoor()
    {
        isCrossingLink = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = data.startPos; 
        Vector3 endPos = data.endPos;

        startPos.y = transform.position.y;
        endPos.y = transform.position.y;

        float currentSpeed = isChasing ? chaseSpeed : speed;

        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;

        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, currentSpeed * Time.deltaTime);
            
            Vector3 direction = (startPos - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            yield return null; 
        }

        transform.position = startPos;

        while (Vector3.Distance(transform.position, endPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, currentSpeed * Time.deltaTime);
            
            Vector3 direction = (endPos - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            yield return null; 
        }

        transform.position = endPos;
        agent.Warp(endPos);

        agent.CompleteOffMeshLink();
        
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        
        isCrossingLink = false;

        if (isChasing)
        {
            agent.SetDestination(lastKnownPlayerPosition);
        }
        else if (isReturning)
        {
            agent.SetDestination(returnPoint);
        }
        else
        {
            if (targets.Length > 0)
            {
                agent.SetDestination(targets[currentTargetIndex].position);
            }
        }
    }
}