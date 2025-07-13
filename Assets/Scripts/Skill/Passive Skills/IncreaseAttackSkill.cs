using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSkill")]
public class IncreaseAttackSkill : Skill
{
    public float attackIncreaseAmount = 5f;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }
}