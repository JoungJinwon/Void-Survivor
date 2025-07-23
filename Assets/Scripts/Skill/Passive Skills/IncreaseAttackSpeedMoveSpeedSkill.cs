using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSpeedMoveSpeedSkill")]
public class IncreaseAttackSpeedMoveSpeedSkill : Skill
{
    public float attackSpeedIncreaseAmount;
    public float moveSpeedIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialAttackSpeedIncreaseAmount = 1.1f;
    [HideInInspector] public float initialMoveSpeedIncreaseAmount = 1.1f;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialAttackSpeedIncreaseAmount = attackSpeedIncreaseAmount;
        initialMoveSpeedIncreaseAmount = moveSpeedIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;
        
        base.ResetToInitialValues();

        attackSpeedIncreaseAmount = initialAttackSpeedIncreaseAmount;
        moveSpeedIncreaseAmount = initialMoveSpeedIncreaseAmount;
    }
}