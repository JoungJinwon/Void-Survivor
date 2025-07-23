using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/MassiveDamageSkill")]
public class MassiveDamageSkill : Skill
{
    private float timer = 0f;
    public float damageAmount;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialDamageAmount;

    public override void Activate()
    {
        base.Activate();

        // 섬광 효과 먼저 실행
        TriggerFlashEffect();
        
        // 섬광과 함께 데미지 적용
        GameManager.Instance.StartCoroutine(ApplyDamageWithDelay());

        cooldownTime = 60f;
        SkillManager.Instance.RegisterUpdate(UpdateSkill);
    }
    
    private void TriggerFlashEffect()
    {
        // 섬광 효과 실행
        if (FlashEffect.Instance != null)
        {
            FlashEffect.Instance.TriggerMassiveFlash();
        }
        
        Debug.Log("Massive Damage Skill - Flash Effect Triggered!");
    }
    
    private IEnumerator ApplyDamageWithDelay()
    {
        // 섬광 효과와 거의 동시에 실행 (약간의 딜레이)
        yield return new WaitForSeconds(0.05f);
        
        foreach (var enemy in PhaseManager.Instance.GetRemainingEnemies())
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
        
        Debug.Log($"Massive Damage applied: {damageAmount} to all enemies!");
    }
    
    private void UpdateSkill()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTime)
        {
            timer = 0f;
            Activate();
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();

        damageAmount *= 1.5f;
    }
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();

        initialDamageAmount = damageAmount;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        damageAmount = initialDamageAmount;
        timer = 0f;
    }
}
