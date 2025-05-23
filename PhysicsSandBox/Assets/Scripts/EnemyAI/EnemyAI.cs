using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector] public EnemyState idleState;
    [HideInInspector] public EnemyState patrolState;
    [HideInInspector] public EnemyState alertState;
    [HideInInspector] public EnemyState chaseState;
    [HideInInspector] public EnemyState attackState;
    [HideInInspector] public EnemyState strafeState;
    [HideInInspector] public EnemyState retreatState;
    // … any others

    [Header("Navigation")] public List<Transform> waypoints;
    public NavMeshAgent agent;
    [Header("Perception")] public Transform player; // assign in inspector or find at runtime
    public float viewDistance = 10f; // how far it can see
    [Range(0, 360)] public float viewAngle = 120f; // field of view
    public LayerMask obstructionMask; // e.g. walls

    [Header("Combat")] public float attackRange = 2f; // melee range
    public float attackCooldown = 1.5f;
    public float backoffDistance = 3f;

    [Header("Animation")] public Animator animator; // your Animator component

    // internal state
    private Transform lastKnownPlayerPos;
    private float lastAttackTime = -Mathf.Infinity;

    private EnemyState currentState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        idleState = new IdleState(this);
        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        strafeState = new StrafeState(this);
        retreatState = new RetreatState(this);
        // … init others

        currentState = idleState;
        currentState.Enter();
    }

    void Update()
    {
        currentState.Tick();
        if (agent.velocity.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(agent.velocity.normalized),
                Time.deltaTime * 10f
            );
        Debug.Log(currentState.ToString());
    }

    public void TransitionToState(EnemyState next)
    {
        currentState.Exit();
        currentState = next;
        currentState.Enter();
    }

    // Utility methods (CanSeePlayer, InAttackRange, Pathfinding, Anim triggers…)
    public bool CanSeePlayer()
    {
        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
        Vector3 dirToPlayer = (player.position - eyePos).normalized;
        float distToPlayer = Vector3.Distance(eyePos, player.position);

        if (distToPlayer > viewDistance)
        {
            Debug.Log($"[Perception] Too far: {distToPlayer:F1} > {viewDistance}");
            return false;
        }

        float angleBetween = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetween > viewAngle * 0.5f)
        {
            Debug.Log($"[Perception] Out of FOV: {angleBetween:F1}° > {viewAngle / 2}°");
            return false;
        }

        if (Physics.Raycast(eyePos, dirToPlayer, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == player)
            {
                lastKnownPlayerPos = hit.transform;
                return true;
            }
            else
            {
                Debug.Log($"[Perception] Ray hit {hit.collider.name}, not player");
            }
        }
        else
        {
            Debug.Log("[Perception] Raycast missed entirely");
        }

        return false;
    }


    public bool InAttackRange()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            return true;
        }

        return false;
    }

    // public void SetAnimation(string stateName)
    // {
    //     // Example using a trigger:
    //     animator.ResetTrigger("Idle");
    //     animator.ResetTrigger("Walk");
    //     animator.ResetTrigger("Run");
    //     animator.ResetTrigger("Attack");
    //     animator.ResetTrigger("Stunned");
    //     // …reset any other triggers…
    //
    //     animator.SetTrigger(stateName);
    // }
    public Transform GetLastKnownPlayerPos()
    {
        return lastKnownPlayerPos;
    }

    public void PatrolWaypoints()
    {
        waypoints ??= new List<Transform>();

        if (waypoints.Count == 0)
        {
            var wpParent = transform.Find("Waypoints");
            if (wpParent != null)
            {
                foreach (Transform child in wpParent)
                    waypoints.Add(child);
            }
        }
    }
    public float BackoffDistance => backoffDistance;
    public void MoveTo(Vector3 target)
    {
        if (agent == null) return;
        agent.SetDestination(target);
    }

    public void MoveTo(Transform target)
    {
        MoveTo(target.position);
    }

    public bool HasReachedDestination()
    {
        if (agent == null || agent.pathPending)
            return false;

        // remainingDistance is updated once the path is computed
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // also check that the agent has actually stopped moving
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                return true;
        }

        return false;
    }

    public bool IsAnimationAtFrame(string chosenAttack, float animationTime)
    {
        float _animationTime = animationTime;
        _animationTime -= Time.deltaTime;
        
        if (_animationTime == .1)
            return true;
        
        return false;
    }

    public bool IsAnimationComplete(string chosenAttack)
    {
        float _animationTime = 0.3f;
        _animationTime -= Time.deltaTime;
        
        if (_animationTime == 0)
            return true;
        
        return true;
    }
}