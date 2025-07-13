using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public int skillId;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public int maxLevel;
    public float cooldownTime;

    public Sprite icon;
    public SkillType skillType;

    public virtual void Activate()
    {
        skillLevel = 1;
        maxLevel = 5;
    }
    
    public abstract void Upgrade();
}

public enum SkillType
{
    Active,     // 플레이어 주 공격 스킬
    SubActive,  // 플레이어 보조 공격 스킬
    Passive     // 플레이어 지속 효과, 능력 상승 스킬
}