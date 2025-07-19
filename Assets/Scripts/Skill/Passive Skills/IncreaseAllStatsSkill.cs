using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAllStatsSkill")]
public class IncreaseAllStatsSkill : Skill
{
    public float healthIncreaseAmount = 30f;
    public int attackIncreaseAmount = 5;
    public float attackSpeedIncreaseAmount = 1.2f;
    public float moveSpeedIncreaseAmount = 1.2f;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }
}