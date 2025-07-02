using UnityEngine;
using static SkillManager;

public abstract class Skill : ScriptableObject
{
    public int skillId;
    public string skillName;
    public string description;
    public int skillLevel;
    public int maxLevel;
    public float cooldownTime;

    public Sprite icon;
    public SkillType skillType;

    public abstract void Activate();
    public abstract void Upgrade();
}
