using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseHealthAttackSkill")]
public class IncreaseHealthAttackSkill : Skill
{
    public float healthIncreaseAmount;
    public int attackIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialHealthIncreaseAmount = 20f;
    [HideInInspector] public int initialAttackIncreaseAmount = 2;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
        GameManager.Instance._Player.IncreaseAttack(attackIncreaseAmount);
    }
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialHealthIncreaseAmount = healthIncreaseAmount;
        initialAttackIncreaseAmount = attackIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        healthIncreaseAmount = initialHealthIncreaseAmount;
        attackIncreaseAmount = initialAttackIncreaseAmount;
    }
}