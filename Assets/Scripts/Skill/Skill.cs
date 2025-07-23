using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [HideInInspector] public bool isMaxLevel;
    public int skillId;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public int maxLevel;
    public float cooldownTime;

    public Sprite icon;
    public SkillType skillType;
    
    // 초기값을 저장하기 위한 변수들
    [HideInInspector] public int initialSkillLevel;
    [HideInInspector] public int initialMaxLevel;
    [HideInInspector] public float initialCooldownTime;
    [HideInInspector] public bool initialIsMaxLevel;
    [HideInInspector] public bool hasStoredInitialValues = false;

    public virtual void Activate()
    {
        // 첫 활성화 시에만 초기값 저장
        if (!hasStoredInitialValues)
        {
            StoreInitialValues();
        }
        
        skillLevel = 1;
    }

    public virtual void Upgrade()
    {
        if (++skillLevel == maxLevel)
        {
            isMaxLevel = true;
            return;
        }
    }
    
    /// <summary>
    /// 현재 ScriptableObject의 값들을 초기값으로 저장합니다
    /// </summary>
    public virtual void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        initialSkillLevel = skillLevel;
        initialMaxLevel = maxLevel;
        initialCooldownTime = cooldownTime;
        initialIsMaxLevel = isMaxLevel;
        hasStoredInitialValues = true;
        
        Debug.Log($"Stored initial values for {skillName}: Level={initialSkillLevel}, MaxLevel={initialMaxLevel}, Cooldown={initialCooldownTime}");
    }
    
    /// <summary>
    /// 스킬을 초기값으로 리셋합니다
    /// </summary>
    public virtual void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;
        
        skillLevel = initialSkillLevel;
        maxLevel = initialMaxLevel;
        cooldownTime = initialCooldownTime;
        isMaxLevel = initialIsMaxLevel;
        
        Debug.Log($"Reset {skillName} to initial values: Level={skillLevel}, MaxLevel={maxLevel}, Cooldown={cooldownTime}");
    }
}

public enum SkillType
{
    Active,     // 플레이어 주 공격 스킬
    SubActive,  // 플레이어 보조 공격 스킬
    Passive     // 플레이어 지속 효과, 능력 상승 스킬
}