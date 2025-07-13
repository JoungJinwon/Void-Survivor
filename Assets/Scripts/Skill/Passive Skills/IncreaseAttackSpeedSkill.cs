using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSpeedSkill")]
public class IncreaseAttackSpeedSkill : Skill
{
    public float attackSpeedIncreaseAmount = 1.2f;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
    }
}