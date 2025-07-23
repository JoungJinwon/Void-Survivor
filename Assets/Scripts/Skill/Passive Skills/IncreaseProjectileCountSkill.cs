using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseProjectileCountSkill")]
public class IncreaseProjectileCountSkill : Skill
{
    Weapon weapon;

    public override void Activate()
    {
        base.Activate();
        
        weapon = GameManager.Instance._Player.GetPlayerWeapon();
        if (weapon == null)
            Debug.LogError($"No weapon fount for {skillName}");
        else
            weapon.IncreaseProjectileCount();
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        weapon.IncreaseProjectileCount();
    }
}