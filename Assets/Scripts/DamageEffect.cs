using UnityEngine;

public class DamageEffect : MonoBehaviour, IPooledObject
{
    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private Vector3 randomForce = new Vector3(1, 1, 1);

    private float timer;

    // 当从对象池取出时调用
    public void OnObjectSpawn()
    {
        timer = lifeTime;
        
        // 重置颜色（以防被修改）
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }

        // 给一个随机的初始速度或方向（如果是刚体）
        // 这里简单模拟：随机偏移一点位置，或者在 Update 里加随机移动
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomZ = Random.Range(-0.5f, 0.5f);
        transform.position += new Vector3(randomX, 0, randomZ);
    }

    private void Update()
    {
        // 向上飘动并旋转
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * 90f * Time.deltaTime);

        // 计时回收
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            // 回收到池中，而不是 Destroy
            ObjectPoolManager.Instance.ReturnToPool("DamageEffect", gameObject);
        }
    }
}
