using UnityEngine;

public interface IEnemyBehavior
{
    void Execute(Enemy enemy, Transform playerTransform);
}