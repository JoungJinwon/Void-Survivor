using UnityEngine;

// Charger type enemy의 기본 행동을 정의하는 클래스
public class ChargerBehaviour : IEnemyBehavior
{
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        Rigidbody _rb = enemy._Rigidbody;
        _rb.MovePosition(_rb.position + directionToPlayer * enemy._EnemyData.moveSpeed * Time.deltaTime);
    }
}
