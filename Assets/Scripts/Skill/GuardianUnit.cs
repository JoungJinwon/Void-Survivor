using UnityEngine;

public class GuardianUnit : MonoBehaviour
{
    private bool hasTarget = false;
    private float unitAttackDamage;
    private float unitCoolTime;
    private float unitLastAttackTime;

    private const float bulletSpeed = 0.8f; // 가디언 총알 속도 조정값
    private Vector3 targetPosition;

    public GameObject guardianBulletPrefab;

    private Player player;
    private Enemy targetEnemy;

    public void Init(float attackDamage, float cooldownTime)
    {
        unitAttackDamage = attackDamage;
        unitCoolTime = cooldownTime;
        unitLastAttackTime = Time.time;

        // Player 참조 초기화
        player = GameManager.Instance._Player;

        Debug.Log($"GuardianUnit Ready!");
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused)
            return;

        UnitMove();

        if (Time.fixedTime - unitLastAttackTime < unitCoolTime)
        {
            return;
        }
        else
        {
            unitLastAttackTime = Time.time;
            UnitAttack();
        }
    }

    private void UnitMove()
    {
        FindEnemy();

        // 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 3f);
    }

    private void UnitAttack()
    {
        if (!hasTarget || targetEnemy == null || !targetEnemy.IsAlive)
        {
            Debug.LogWarning("Guardian: 타겟이 없으므로 공격을 취소합니다.");
            return;
        }

        // 타겟을 향한 방향 계산
        Vector3 attackDirection = (targetEnemy.transform.position - transform.position).normalized;

        GameObject bullet = Instantiate(guardianBulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.Init(attackDirection, bulletSpeed, (int)unitAttackDamage);
        }
    }

    private void FindEnemy()
    {
        if (player == null)
            player = GameManager.Instance._Player;

        // player가 타겟을 가지고 있으면 그것을 우선 사용
        if (player.targetEnemy != null && player.targetEnemy.IsAlive)
        {
            targetEnemy = player.targetEnemy;
            hasTarget = true;
            return;
        }

        // player가 타겟을 가지고 있지 않거나 타겟이 죽었으면 직접 찾는다
        float minDist = float.MaxValue;
        Enemy closestEnemy = null;

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            hasTarget = true;
        }
        else
        {
            targetEnemy = null;
            hasTarget = false;
        }
    }
}
