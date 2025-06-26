using UnityEngine;

public interface IAttackable
{
    float AttackDamage { get; set; }
    float AttackSpeed { get; set; }
    void Attack();
}
