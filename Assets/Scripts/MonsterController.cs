using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("References")]
    [SerializeField] private Transform target; // Player

    private State currentState;
    private float lastAttackTime;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        // 如果未指定目标，尝试查找 Player 标签的对象
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToTarget <= detectionRange)
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                if (distanceToTarget <= attackRange)
                {
                    currentState = State.Attack;
                }
                else if (distanceToTarget > detectionRange * 1.5f) // 失去目标
                {
                    currentState = State.Idle;
                }
                else
                {
                    MoveTowardsTarget();
                }
                break;

            case State.Attack:
                if (distanceToTarget > attackRange)
                {
                    currentState = State.Chase;
                }
                else
                {
                    TryAttack();
                }
                break;
        }
    }

    private void MoveTowardsTarget()
    {
        // 简单的朝向移动，不依赖 NavMesh
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // 保持在地面高度（假设平坦地形）
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log($"{gameObject.name} attacks {target.name}!");
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
