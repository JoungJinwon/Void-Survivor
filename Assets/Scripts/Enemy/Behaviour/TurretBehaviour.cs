using UnityEngine;

public class TurretBehaviour : IEnemyBehavior
{
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        if (GameManager.Instance.IsGamePaused)
            return;
        // Shooter Behaviour logic 추가

    }
}
