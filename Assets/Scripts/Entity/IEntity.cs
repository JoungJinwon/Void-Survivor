using UnityEngine;

public interface IEntity
{
    void TakeDamage(float amount);
    void Attack(IEntity target);
    bool IsAlive { get; }
    Vector3 Position { get; }
}
