using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject damageEffectPrefab; // 红色方块 Prefab
    [SerializeField] private int effectCountPerHit = 5; // 每次攻击掉落数量

    private void Start()
    {
        // 初始化对象池
        if (damageEffectPrefab == null)
        {
            // 尝试查找原型
            GameObject proto = GameObject.Find("DamageEffectPrototype");
            if (proto != null)
            {
                damageEffectPrefab = proto;
            }
            else
            {
                // 如果还是没有，创建一个临时的 Prefab
                damageEffectPrefab = CreateFallbackPrefab();
            }
        }

        // 确保 ObjectPoolManager 存在
        if (ObjectPoolManager.Instance == null)
        {
            GameObject poolManager = new GameObject("ObjectPoolManager");
            poolManager.AddComponent<ObjectPoolManager>();
        }

        // 创建池 (预热 50 个)
        if (damageEffectPrefab != null)
        {
            ObjectPoolManager.Instance.CreatePool("DamageEffect", damageEffectPrefab, 50);
        }
    }

    private GameObject CreateFallbackPrefab()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "FallbackDamageEffect";
        cube.transform.localScale = Vector3.one * 0.2f; // 更小一点
        cube.GetComponent<Renderer>().material.color = Color.red;
        
        // 移除碰撞体
        Collider col = cube.GetComponent<Collider>();
        if (col != null) DestroyImmediate(col);
        
        // 添加脚本
        if (cube.GetComponent<DamageEffect>() == null) cube.AddComponent<DamageEffect>();
        
        // 设为非激活并作为 Prefab 使用（虽然它是在场景里的物体，但 Instantiate 可以用）
        cube.SetActive(false);
        return cube;
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

        int layerMask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1f, attackRange, layerMask);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject) continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                
                // 生成多个特效
                SpawnDamageEffects(hitCollider.transform.position + Vector3.up);
            }
        }
    }

    private void SpawnDamageEffects(Vector3 position)
    {
        for (int i = 0; i < effectCountPerHit; i++)
        {
            ObjectPoolManager.Instance.SpawnFromPool("DamageEffect", position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, attackRange);
    }
}
