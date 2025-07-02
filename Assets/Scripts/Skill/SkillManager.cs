using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public enum SkillType
    {
        Active,     // 플레이어 주 공격 스킬
        SubActive,  // 플레이어 보조 공격 스킬
        Passive     // 플레이어 지속 효과, 능력 상승 스킬
    }

    public List<Skill> skillDatabase = new List<Skill>();
    public List<Skill> selectableSkills = new List<Skill>();
    public List<Skill> equippedSkills = new List<Skill>();

    private void Awake()
    {
        // 게임 시작시 selectableSkills를 skillDatabase로 초기화
        selectableSkills.Clear();
        selectableSkills.AddRange(skillDatabase);

        equippedSkills.Clear();
    }

    private void Start()
    {
        // 게임 시작 시 BulletSkill 자동 장착
        foreach (var skill in skillDatabase)
        {
            if (skill is BulletSkill)
            {
                AddSkill(skill);
                break;
            }
        }
    }

#region Skill Management
    public void AddSkill(Skill skill)
    {
        equippedSkills.Add(skill);
        selectableSkills.Remove(skill);
        skill.Activate();
    }

    public void UpgradeSkill(Skill skill)
    {
        skill.Upgrade();
    }

    public List<Skill> GetNewSkill()
    {
        List<Skill> newSkills = new List<Skill>();
        // selectableSkills에서 3개 랜덤 선택
        List<Skill> tempList = new List<Skill>(selectableSkills);
        int count = Mathf.Min(3, tempList.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, tempList.Count);
            newSkills.Add(tempList[idx]);
            tempList.RemoveAt(idx);
        }
        return newSkills;
    }
#endregion
}
