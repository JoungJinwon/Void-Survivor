using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
    private int damage;

    public void Init(Vector3 direction, float speed, int damage)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        this.damage = damage;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy 태그가 붙은 오브젝트와 충돌 시 데미지 적용
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (enemy.IsAlive)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // 벽 등 다른 오브젝트와 충돌 시에도 파괴
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
