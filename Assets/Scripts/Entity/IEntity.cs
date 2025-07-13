using UnityEngine;

public interface IEntity
{
    void TakeDamage(float amount);
    bool IsAlive { get; }
    Vector3 Position { get; }
}
