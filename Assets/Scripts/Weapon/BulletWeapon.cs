using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Weapon", menuName = "My Scriptable Objects/Weapons/Bullet Weapon")]
public class BulletWeapon : Weapon
{
    private float cooldownTime = 2.0f;

    [SerializeField] private GameObject bulletPrefab;

    public override void InitWeapon()
    {
        base.InitWeapon();
        attackIntervalMultiplier = 1f;
    }

    public override void UpgradeWeapon()
    {
        if (weaponLevel >= maxWeaponLevel)
        {
            Debug.LogWarning($"{this} Weapon is already at MAXIMUM level!");
            return;
        }
        else
        {
            weaponLevel++;
            attackDamage += 5f;
            attackIntervalMultiplier *= 1.1f;
        }
    }

    public override void IncreaseProjectileCount()
    {
        projectileCount++;
    }

    public override void TryAttack(Player player)
    {
        float weaponSpeed = attackIntervalMultiplier * player.GetPlayerAttackSpeed();
        float effectiveAttackTime = (Time.time - lastAttackTime) * weaponSpeed; // 스킬 경과 시간에 공격 속도를 적용
        if (effectiveAttackTime < cooldownTime) return;

        // Find nearest enemy in range
        Enemy nearestEnemy = null;
        float minDist = float.MaxValue;
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < minDist && enemy.IsAlive)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            // 다중 발사체 생성
            Vector3 directionToEnemy = (nearestEnemy.transform.position - player.transform.position).normalized;
            
            for (int i = 0; i < projectileCount; i++)
            {
                // 발사 방향을 기준으로 수직 방향으로 오프셋 계산
                float offset = (i - (projectileCount - 1) / 2f) * 0.5f; // 0.5f는 발사체 간 간격
                Vector3 rightVector = Vector3.Cross(directionToEnemy, Vector3.up).normalized;
                Vector3 startPosition = player.transform.position + rightVector * offset;
                
                FireBullet(startPosition, nearestEnemy.transform.position, weaponSpeed);
            }
            
            lastAttackTime = Time.time;

            Debug.Log($"Bullet Weapon: Fire {projectileCount} bullets to {nearestEnemy}!");
        }
        else
        {
            Debug.Log("Bullet Weapon: No enemies in range to attack.");
        }
    }

    private void FireBullet(Vector3 origin, Vector3 target, float bulletSpeed)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.identity);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.Init(target - origin, attackIntervalMultiplier, (int)attackDamage);
        }
    }
}
