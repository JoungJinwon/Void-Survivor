using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Skill", menuName = "My Scriptable Objects/Skills/Bullet Skill")]
public class BulletSkill : Skill
{
    public int damage;
    public float bulletSpeed = 30.0f;
    public GameObject bulletPrefab;

    private float lastAttackTime;

    public override void Activate()
    {
        skillId = 0;
        skillName = "Bullet Skill";
        description = "에너지탄으로 적들을 공격합니다";
        skillLevel = 1;
        maxLevel = 5;
        cooldownTime = 2.0f;
        lastAttackTime = Time.time;
        Debug.Log($"Skill Activating {skillName}");
    }

    public override void Upgrade()
    {
        skillLevel++;
        bulletSpeed += 1.0f;
        damage += 5;
        Debug.Log($"Upgraded {skillName} to level {maxLevel}, speed {bulletSpeed}, damage {damage}");
    }

    public void TryAttack(Player player)
    {
        if (Time.time - lastAttackTime < cooldownTime) return;

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
            FireBullet(player.transform.position, nearestEnemy.transform.position);
            lastAttackTime = Time.time;
        }
        else
        {
            Debug.Log("Bullet Skill: No enemies in range to attack.");
        }
    }

    private void FireBullet(Vector3 origin, Vector3 target)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.identity);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.Init(target - origin, bulletSpeed, damage);
            Debug.Log("Bullet Skill: Fire bullet!");
        }
    }
}