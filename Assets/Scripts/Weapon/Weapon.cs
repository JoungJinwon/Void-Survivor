using UnityEngine;
using UnityEngine.Rendering;

public abstract class Weapon : ScriptableObject
{
    protected int weaponLevel = 1;
    protected int maxWeaponLevel = 10;
    protected int projectileCount = 1;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackIntervalMultiplier = 1f;
    protected float lastAttackTime;

    public virtual void InitWeapon() { lastAttackTime = Time.time; }
    public abstract void UpgradeWeapon();

    public abstract void IncreaseProjectileCount();

    // 플레이어 공격 속도를 인자로 받음
    public abstract void TryAttack(Player player);
    
}
