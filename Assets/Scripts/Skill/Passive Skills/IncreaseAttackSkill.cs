using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseAttackSkill")]
public class IncreaseAttackSkill : Skill
{
    public int attackIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public int initialAttackIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }
    
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialAttackIncreaseAmount = attackIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        attackIncreaseAmount = initialAttackIncreaseAmount;
    }
}