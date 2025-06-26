using UnityEngine;

public class TurretBehaviour : IEnemyBehavior
{
    public void Execute(Enemy enemy, Transform playerTransform)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, playerTransform.position);
        
        if (distanceToPlayer <= enemy._EnemyData.attackRange)
        {
            Debug.Log("Enemy(터렛형)는 공격 중!");
        }
    }
}
