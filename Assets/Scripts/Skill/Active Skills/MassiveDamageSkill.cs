using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/MassiveDamageSkill")]
public class MassiveDamageSkill : Skill
{
    public float damageAmount;

    public override void Activate()
    {
        base.Activate();
        
        foreach (var enemy in PhaseManager.Instance.GetRemainingEnemies())
        {
            enemy.TakeDamage(damageAmount);
        }
    }

    public override void Upgrade()
    {
        skillLevel++;
        damageAmount *= 1.2f;
    }
}
