using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;

    private int level = 1;
    private float currentHealth;
    private float lastAttackTime;

    [SerializeField] private Slider hpBar; // Player의 체력바를 표시할 Slider UI

    [SerializeField]
    private SkillManager skillManager;

    void Start()
    {
        currentHealth = maxHealth;
        IsAlive = true;

        if (skillManager == null)
            skillManager = FindFirstObjectByType<SkillManager>();
    }

    void Update()
    {
        if (!IsAlive) return;
        // 플레이어 입력 처리 및 이동 로직

        // 장착된 스킬 자동 사용
        Skill skill = skillManager.equippedSkills[0];
        if (skill is BulletSkill bulletSkill)
        {
            Debug.Log($"Using skill: {bulletSkill.skillName}");
            bulletSkill.TryAttack(this);
        }
    }

    public override void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (hpBar != null)
            hpBar.value = currentHealth / maxHealth;

        if (currentHealth <= 0 && IsAlive)
        {
            Die();
        }
    }

    public override void Attack(IEntity target)
    {
        if (Time.time - lastAttackTime < attackSpeed) return;

        target.TakeDamage(attackDamage);
        lastAttackTime = Time.time;
    }

    protected override void Die()
    {
        IsAlive = false;
        // 게임 오버 처리
    }
}
