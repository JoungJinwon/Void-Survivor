using UnityEngine;

public class TurretBehaviour : IEnemyBehavior
{
    private float lastAttackTime;
    
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        if (GameManager.Instance.IsGamePaused)
            return;

        // Turret은 고정된 위치에서 플레이어를 향해 투사체 발사
        Player player = GameManager.Instance._Player;
        if (player == null) return;
        
        // 공격 속도에 따라 발사
        if (Time.time - lastAttackTime >= enemy._EnemyData.attackCoolTime)
        {
            FireBullet(enemy, directionToPlayer);
            lastAttackTime = Time.time;
        }
    }
    
    private void FireBullet(Enemy enemy, Vector3 direction)
    {
        // TurretEnemyData에서 총알 프리팹 가져오기
        if (enemy._EnemyData is TurretEnemyData turretData && turretData.enemyBulletPrefab != null)
        {
            GameObject bullet = Object.Instantiate(turretData.enemyBulletPrefab, enemy.transform.position, Quaternion.identity);
            
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
            Debug.LogWarning($"TurretBehaviour: Enemy '{enemy.name}'의 EnemyData가 TurretEnemyData가 아니거나 enemyBulletPrefab이 설정되지 않았습니다.");
        }
    }
}
