using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseHealthSkill")]
public class IncreaseHealthSkill : Skill
{
    public float healthIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialHealthIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
    }
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialHealthIncreaseAmount = healthIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        healthIncreaseAmount = initialHealthIncreaseAmount;
    }
}