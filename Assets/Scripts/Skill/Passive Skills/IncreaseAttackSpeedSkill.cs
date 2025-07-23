using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSpeedSkill")]
public class IncreaseAttackSpeedSkill : Skill
{
    public float attackSpeedIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialAttackSpeedIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();

        GameManager.Instance._Player.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
    }
    
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialAttackSpeedIncreaseAmount = attackSpeedIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        attackSpeedIncreaseAmount = initialAttackSpeedIncreaseAmount;
    }
}