using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseMoveSpeedSkill")]
public class IncreaseMoveSpeedSkill : Skill
{
    public float moveSpeedIncreaseAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialMoveSpeedIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();
        
        initialMoveSpeedIncreaseAmount = moveSpeedIncreaseAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        moveSpeedIncreaseAmount = initialMoveSpeedIncreaseAmount;
    }
}