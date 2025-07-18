using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // 전체 스킬 List
    public List<Skill> skillDatabase = new List<Skill>();
    // 선택 가능한 스킬 List
    public List<Skill> selectableSkills = new List<Skill>();
    // 플레이어에게 장착된 스킬 List
    public List<Skill> equippedSkills = new List<Skill>();

    public List<Skill> chosenSkills = new List<Skill>();

    // 업데이트 델리게이트 리스트 추가
    private List<System.Action> updateActions = new List<System.Action>();

    private void Awake()
    {
        // 게임 시작시 selectableSkills를 skillDatabase로 초기화
        selectableSkills.Clear();
        selectableSkills.AddRange(skillDatabase);

        equippedSkills.Clear();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        // 등록된 모든 업데이트 델리게이트 호출
        for (int i = 0; i < updateActions.Count; i++)
        {
            updateActions[i]?.Invoke();
        }
    }

    // 외부에서 업데이트 델리게이트 등록
    public void RegisterUpdate(System.Action updateAction)
    {
        if (updateAction != null && !updateActions.Contains(updateAction))
        {
            updateActions.Add(updateAction);
        }
    }

    // 외부에서 업데이트 델리게이트 해제
    public void UnregisterUpdate(System.Action updateAction)
    {
        if (updateAction != null && updateActions.Contains(updateAction))
        {
            updateActions.Remove(updateAction);
        }
    }

    #region Skill Management
    public bool HasSkill(Skill skill)
    {
        return equippedSkills.Contains(skill);
    }

    public void AddSkill(Skill skill)
    {
        equippedSkills.Add(skill);
        selectableSkills.Remove(skill);
        skill.Activate();

        UiManager.Instance.UpdateEquippedSkillsGrid(skill);
    }

    public void UpgradeSkill(Skill skill)
    {
        skill.Upgrade();
    }

    public void SelectSkill(int skillIdx)
    {
        Skill selectedSkill = chosenSkills[skillIdx];

        if (HasSkill(selectedSkill))
        {
            UpgradeSkill(selectedSkill);
            Debug.Log($"Skill Manager: {selectedSkill.skillName} 업그레이드!");
        }
        else
        {
            AddSkill(selectedSkill);
            Debug.Log($"Skill Manager: {selectedSkill.skillName} 획득!");
        }

        chosenSkills.Clear();

        // 스킬 선택 완료 후 게임 재개
        if (UiManager.Instance != null)
        {
            UiManager.Instance.OnSkillSelected();
        }
    }

    // selectable Skills 리스트에서 3개(새 스킬이 3개 미만이면 해당 개수)만큼의 스킬 리스트를 반환해준다.
    public List<Skill> GetNewSkills()
    {
        // selectableSkills에서 3개 랜덤 선택
        List<Skill> candidateSkillList = new List<Skill>(selectableSkills);
        int count = Mathf.Min(3, candidateSkillList.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, candidateSkillList.Count);
            chosenSkills.Add(candidateSkillList[idx]);
            candidateSkillList.RemoveAt(idx);
        }

        return chosenSkills;
    }
    #endregion
}
