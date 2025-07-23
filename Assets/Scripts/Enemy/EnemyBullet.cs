using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private const int DefaultBulletSpeed = 20;
    private Vector3 moveDirection;
    private float moveSpeed;
    private int damage;

    public void Init(Vector3 direction, float speed, int damage)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed * DefaultBulletSpeed;
        this.damage = damage;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 5초 후 자동 파괴
        Destroy(gameObject, 7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player 태그가 붙은 오브젝트와 충돌 시 데미지 적용
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                player.TakeDamage(damage);
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
