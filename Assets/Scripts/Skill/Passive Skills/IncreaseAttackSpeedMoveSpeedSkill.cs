using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSpeedMoveSpeedSkill")]
public class IncreaseAttackSpeedMoveSpeedSkill : Skill
{
    public float attackSpeedIncreaseAmount = 1.2f;
    public float moveSpeedIncreaseAmount = 1.2f;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }
}   