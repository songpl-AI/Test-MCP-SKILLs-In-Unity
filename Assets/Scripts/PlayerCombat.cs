using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject damageEffectPrefab; // 红色方块 Prefab

    private void Start()
    {
        if (damageEffectPrefab == null)
        {
            // 尝试在场景中查找名为 "DamageEffectPrototype" 的物体
            // 注意：如果它被隐藏（SetActive(false)），GameObject.Find 可能找不到。
            // 我们可以创建一个专用的资源加载逻辑，或者暂时要求它在场景中激活但放在视野外。
            GameObject proto = GameObject.Find("DamageEffectPrototype");
            if (proto != null)
            {
                damageEffectPrefab = proto;
            }
            else
            {
                Debug.LogWarning("PlayerCombat: DamageEffectPrefab is missing and 'DamageEffectPrototype' not found in scene.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left mouse click or Ctrl
        {
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("Player attacks!");

        // 简单的前方球形检测
        // 注意：这里假设怪物在 "Default" 层或者特定的 Enemy 层，如果没有设置 LayerMask，可能会检测到自己
        // 如果 enemyLayers 为 0 (Nothing)，Physics.OverlapSphere 可能会行为异常，最好默认设为 Everything
        int layerMask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1f, attackRange, layerMask);
        
        foreach (var hitCollider in hitColliders)
        {
            // 避免打到自己
            if (hitCollider.gameObject == gameObject) continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                
                // 生成特效
                if (damageEffectPrefab != null)
                {
                    Instantiate(damageEffectPrefab, hitCollider.transform.position + Vector3.up, Quaternion.identity);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, attackRange);
    }
}
