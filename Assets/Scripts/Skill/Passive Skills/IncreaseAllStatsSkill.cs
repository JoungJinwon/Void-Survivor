using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAllStatsSkill")]
public class IncreaseAllStatsSkill : Skill
{
    public float healthIncreaseAmount;
    public int attackIncreaseAmount;
    public float attackSpeedIncreaseAmount;
    public float moveSpeedIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialHealthIncreaseAmount;
    [HideInInspector] public int initialAttackIncreaseAmount;
    [HideInInspector] public float initialAttackSpeedIncreaseAmount;
    [HideInInspector] public float initialMoveSpeedIncreaseAmount;

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
        base.Upgrade();

        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialHealthIncreaseAmount = healthIncreaseAmount;
        initialAttackIncreaseAmount = attackIncreaseAmount;
        initialAttackSpeedIncreaseAmount = attackSpeedIncreaseAmount;
        initialMoveSpeedIncreaseAmount = moveSpeedIncreaseAmount;
    }

    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        healthIncreaseAmount = initialHealthIncreaseAmount;
        attackIncreaseAmount = initialAttackIncreaseAmount;
        attackSpeedIncreaseAmount = initialAttackSpeedIncreaseAmount;
        moveSpeedIncreaseAmount = initialMoveSpeedIncreaseAmount;
    }
}