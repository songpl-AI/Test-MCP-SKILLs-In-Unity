using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float floatSpeed = 2f;

    private void Start()
    {
        // 自动销毁
        Destroy(gameObject, lifeTime);
        
        // 确保是红色的
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }
    }

    private void Update()
    {
        // 向上飘动并旋转
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * 90f * Time.deltaTime);
    }
}
