using UnityEngine;

public class ShooterBehaviour : IEnemyBehavior
{
    public void Execute(Enemy enemy, Transform playerTransform)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, playerTransform.position);
        
        if (distanceToPlayer <= enemy._EnemyData.detectionRange)
        {
            Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
            enemy.Move(direction);
            
            if (distanceToPlayer <= enemy._EnemyData.attackRange)
            {
                Debug.Log("Enemy(슈터형)는 공격 중!");
            }
        }
    }
}
