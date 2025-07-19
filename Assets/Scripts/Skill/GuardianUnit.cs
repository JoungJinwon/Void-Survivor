using UnityEngine;

public class GuardianUnit : MonoBehaviour
{
    private bool hasTarget = false;
    private float unitAttackDamage;
    private float unitCoolTime;
    private float unitLastAttackTime;

    // 유닛 회전 속도 (Degree per second)
    private float unitMoveSpeed = 45f;
    private float playerAttachedDistance = 5f;

    private const float bulletSpeed = 30f; // 총알 속도

    private Vector3 playerToTargetDir;
    private Vector3 playerToUnitDir;
    private Vector3 unitAttackDir;

    public GameObject guardianBulletPrefab;

    private Player player;
    private Enemy targetEnemy;

    public void Init(float attackDamage, float cooldownTime)
    {
        unitAttackDamage = attackDamage;
        unitCoolTime = cooldownTime;
        unitLastAttackTime = Time.time;

        Debug.Log($"GuardianUnit Ready!");
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused)
            return;

        UnitMove();

        if (Time.time - unitLastAttackTime < unitCoolTime)
            return;
        else
        {
            unitLastAttackTime = Time.time;
            UnitAttack();
        }
    }

    private void UnitMove()
    {
        FindEnemy();

        Vector3 playerPos = player.transform.position;

        if (!hasTarget)
        {
            // No enemies - maintain current position around player
            Vector3 currentDir = (transform.position - playerPos).normalized;
            transform.position = playerPos + currentDir * playerAttachedDistance;
            return;
        }

        playerToTargetDir = targetEnemy.transform.position - playerPos;
        playerToUnitDir = transform.position - playerPos;
        unitAttackDir = (playerToTargetDir - playerToUnitDir).normalized;
        Vector3 desiredDirection = -playerToTargetDir.normalized;
        playerToUnitDir = playerToUnitDir.normalized;

        float currentAngle = Mathf.Atan2(playerToUnitDir.z, playerToUnitDir.x);
        float targetAngle = Mathf.Atan2(desiredDirection.z, desiredDirection.x);

        float rotationSpeed = unitMoveSpeed * Time.fixedDeltaTime * Mathf.Deg2Rad;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed);

        transform.position = playerPos + new Vector3(
            Mathf.Cos(newAngle) * playerAttachedDistance,
            0f,
            Mathf.Sin(newAngle) * playerAttachedDistance
        );
    }

    private void UnitAttack()
    {
        if (playerToTargetDir == Vector3.zero || !hasTarget) return;

        GameObject bullet = Instantiate(guardianBulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.Init(unitAttackDir, bulletSpeed, (int)unitAttackDamage);
        }
    }

    private void FindEnemy()
    {
        if (player == null)
            player = GameManager.Instance._Player;

        // player가 타겟을 가지고 있지 않으면 직접 찾는다
        if (player.targetEnemy == null)
        {
            float minDist = float.MaxValue;

            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (enemies.Length == 0)
            {
                targetEnemy = null;
                hasTarget = false;
                return; // 없다면 바로 return
            }
            else
            {
                foreach (Enemy enemy in enemies)
                {
                    float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
                    if (dist < minDist && enemy.IsAlive)
                    {
                        minDist = dist;
                        targetEnemy = enemy;
                    }
                }

                hasTarget = true;
            }
        }
        else
            targetEnemy = player.targetEnemy;
    }
}
