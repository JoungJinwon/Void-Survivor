using UnityEngine;
using UnityEngine.Rendering;

public abstract class Weapon : ScriptableObject
{
    protected int weaponLevel = 1;
    protected int maxWeaponLevel = 10;

    public int projectileCount = 1;

    [SerializeField] protected float attackDamage = 10f;
    // 공격 속도 조정을 위한 변수. 커질수록 공격 속도가 빨라짐
    protected float attackIntervalMultiplier = 1f;
    protected float lastAttackTime;

    public AudioClip attackSound;

    public virtual void InitWeapon() { lastAttackTime = Time.time; }
    public abstract void UpgradeWeapon();

    public abstract void IncreaseProjectileCount();

    // 플레이어 공격 속도를 인자로 받음
    public abstract void TryAttack(Player player);
    
}
