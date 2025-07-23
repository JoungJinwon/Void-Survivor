using UnityEngine;
using System.Runtime.CompilerServices;

public class EnemyBullet : MonoBehaviour
{
    private const int DefaultBulletSpeed = 20;
    private const float BulletLifeTime = 10f;

    private int bulletdamage;
    private float bulletSpeed;
    private float remainingLifeTime = BulletLifeTime;

    private Vector3 bulletDirection;

    public void Init(Vector3 direction, float speed, int damage)
    {
        bulletDirection = direction.normalized;
        bulletSpeed = speed * DefaultBulletSpeed;
        bulletdamage = damage;
    }
    
    private void Update()
    {
        if (GameManager.Instance.IsGamePaused)
            return;

        remainingLifeTime -= Time.deltaTime;
        if (remainingLifeTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        MoveBullet();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveBullet()
    {
        transform.position += bulletDirection * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                player.TakeDamage(bulletdamage);
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
