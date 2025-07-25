using UnityEngine;

public class ShooterBehaviour : IEnemyBehavior
{
    private float lastAttackTime;
    
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        if (GameManager.Instance.IsGamePaused)
            return;
        
        Player player = GameManager.Instance._Player;
        if (player == null) return;
        
        float playerDistance = Vector3.Distance(enemy.transform.position, player.transform.position);
        
        // 플레이어가 공격 범위 내에 있으면 투사체 발사
        if (playerDistance <= enemy._EnemyData.detectionRange)
        {
            // 공격 속도에 따라 발사
            if (Time.time - lastAttackTime >= enemy._EnemyData.attackCoolTime)
            {
                FireBullet(enemy, directionToPlayer);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // 플레이어가 사정거리 밖에 있으면 charger처럼 이동
            Rigidbody rb = enemy._Rigidbody;
            rb.MovePosition(rb.position + directionToPlayer * enemy._EnemyData.moveSpeed * Time.deltaTime);
        }
    }
    
    private void FireBullet(Enemy enemy, Vector3 direction)
    {
        // ShooterEnemyData에서 총알 프리팹 가져오기
        if (enemy._EnemyData is ShooterEnemyData shooterData && shooterData.enemyBulletPrefab != null)
        {
            GameObject bullet = Object.Instantiate(shooterData.enemyBulletPrefab, enemy.transform.position, Quaternion.identity);
            
            // EnemyBullet 컴포넌트가 있으면 사용, 없으면 일반 Bullet 사용
            if (bullet.TryGetComponent(out EnemyBullet enemyBullet))
            {
                enemyBullet.Init(direction, 1f, (int)enemy._EnemyData.attackDamage);
            }
            else if (bullet.TryGetComponent(out Bullet normalBullet))
            {
                normalBullet.Init(direction, 1f, (int)enemy._EnemyData.attackDamage);
            }
        }
        else
        {
            Debug.LogWarning($"ShooterBehaviour: Enemy '{enemy.name}'의 EnemyData가 ShooterEnemyData가 아니거나 enemyBulletPrefab이 설정되지 않았습니다.");
        }
    }
}
