using UnityEngine;
using UnityEngine.AI;
using Turok26.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBrain : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public float runSpeed = 7f;
    public float damage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Detection")]
    public float detectionRange = 20f;
    public float fovAngle = 90f;
    public float loseInterestRange = 40f;

    [Header("Patrol")]
    public float patrolRadius = 15f;
    public float idleDurationMin = 2f;
    public float idleDurationMax = 5f;
    public float waitAtPoint = 3f;

    [Header("Flee")]
    public float fleeHealthThreshold = 0.25f;
    public float fleeDistance = 30f;

    [Header("Combat")]
    public float retreatDistance = 5f;
    public float strafeInterval = 2f;

    [Header("Loot & XP")]
    public LootDropper lootDropper;
    public int xpReward = 25;

    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

    protected NavMeshAgent agent;
    protected Transform target;
    protected float currentHealth;
    protected float stateTimer;
    protected float attackTimer;
    protected Vector3 homePosition;
    protected Vector3 patrolTarget;
    protected Animator animator;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        homePosition = transform.position;
        agent.stoppingDistance = attackRange * 0.8f;
    }

    protected virtual void Start()
    {
        SetState(EnemyState.Idle);
    }

    protected virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        switch (CurrentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Alert: UpdateAlert(); break;
            case EnemyState.Combat: UpdateCombat(); break;
            case EnemyState.Flee: UpdateFlee(); break;
        }

        UpdateAnimator();
    }

    protected void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("speed", agent.velocity.magnitude / runSpeed);
    }

    public void SetState(EnemyState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case EnemyState.Idle:
                stateTimer = Random.Range(idleDurationMin, idleDurationMax);
                agent.isStopped = true;
                break;
            case EnemyState.Patrol:
                PickPatrolTarget();
                agent.isStopped = false;
                agent.speed = moveSpeed;
                agent.SetDestination(patrolTarget);
                break;
            case EnemyState.Alert:
                stateTimer = 1.5f;
                agent.isStopped = true;
                if (target != null) transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                break;
            case EnemyState.Combat:
                agent.isStopped = false;
                agent.speed = runSpeed;
                if (target != null) agent.SetDestination(target.position);
                break;
            case EnemyState.Flee:
                agent.isStopped = false;
                agent.speed = runSpeed * 1.2f;
                FleeFromTarget();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
        target = FindTarget();
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= detectionRange)
            {
                SetState(EnemyState.Alert);
                return;
            }
        }
        if (stateTimer <= 0f) SetState(EnemyState.Patrol);
    }

    protected virtual void UpdatePatrol()
    {
        target = FindTarget();
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= detectionRange)
            {
                SetState(EnemyState.Alert);
                return;
            }
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            SetState(EnemyState.Idle);
    }

    protected virtual void UpdateAlert()
    {
        target = FindTarget();
        if (target == null)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange * 1.5f)
            SetState(EnemyState.Combat);
        else if (dist > loseInterestRange)
            SetState(EnemyState.Idle);
        else if (stateTimer <= 0f) SetState(EnemyState.Combat);
    }

    protected virtual void UpdateCombat()
    {
        target = FindTarget();
        if (target == null)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > loseInterestRange)
        {
            SetState(EnemyState.Idle);
            return;
        }

        if (currentHealth / maxHealth <= fleeHealthThreshold)
        {
            SetState(EnemyState.Flee);
            return;
        }

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    protected virtual void UpdateFlee()
    {
        if (agent.pathPending || agent.remainingDistance > 1f) return;

        target = FindTarget();
        float dist = target != null ? Vector3.Distance(transform.position, target.position) : 100f;

        if (dist > fleeDistance || target == null)
            SetState(EnemyState.Idle);
        else
            FleeFromTarget();
    }

    protected virtual Transform FindTarget()
    {
        GameObject player = GameObject.FindWithTag("Player");
        return player != null ? player.transform : null;
    }

    protected bool IsTargetInFOV()
    {
        if (target == null) return false;
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle <= fovAngle * 0.5f;
    }

    protected void PickPatrolTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
        randomDir.y = 0;
        patrolTarget = homePosition + randomDir;

        if (NavMesh.SamplePosition(patrolTarget, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            patrolTarget = hit.position;
    }

    protected void FleeFromTarget()
    {
        if (target == null) return;
        Vector3 dir = (transform.position - target.position).normalized;
        Vector3 fleePos = transform.position + dir * fleeDistance;

        if (NavMesh.SamplePosition(fleePos, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(transform.position + dir * fleeDistance * 0.5f);
    }

    protected virtual void Attack()
    {
        if (target == null) return;

        if (animator != null)
            animator.SetTrigger("attack");

        IHittable hittable = target.GetComponent<IHittable>();
        if (hittable != null)
            hittable.TakeDamage(damage);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (animator != null)
            animator.SetTrigger("hit");

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Patrol)
            SetState(EnemyState.Alert);
    }

    protected virtual void Die()
    {
        SetState(EnemyState.Dead);
        agent.isStopped = true;
        agent.enabled = false;
        if (animator != null)
            animator.SetBool("dead", true);

        SpawnLoot();
        GrantXPToKiller();

        enabled = false;
    }

    protected virtual void SpawnLoot()
    {
        if (lootDropper != null)
            lootDropper.SpawnLoot(transform.position);
    }

    protected virtual void GrantXPToKiller()
    {
        if (target == null) return;

        var level = target.GetComponent<CharacterLevel>();
        if (level != null)
            level.AddXP(xpReward);
    }
}
