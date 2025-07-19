using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseHealthAttackSkill")]
public class IncreaseHealthAttackSkill : Skill
{
    public float healthIncreaseAmount;
    public int attackIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }
}