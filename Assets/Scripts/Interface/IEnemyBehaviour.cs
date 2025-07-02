using UnityEngine;

public interface IEnemyBehavior
{
    void Execute(Enemy enemy, Vector3 directionToPlayer);
}