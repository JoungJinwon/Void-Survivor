using UnityEngine;

// <summary>
// 게임 내 플레이어 및 적의 기본적인 속성과 행동을 정의하는 모든 캐릭터의 부모 클래스
public abstract class Entity : MonoBehaviour, IEntity
{
    public bool IsAlive { get; protected set; } = true;
    public Vector3 Position => transform.position;

    public abstract void TakeDamage(float amount);
    public abstract void Attack(IEntity target);
    protected abstract void Die();
}