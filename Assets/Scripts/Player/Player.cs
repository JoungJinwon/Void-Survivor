using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;
    
    private float currentHealth;
    private float lastAttackTime;

    [SerializeField] private Slider hpBar; // Player의 체력바를 표시할 Slider UI

    void Start()
    {
        currentHealth = maxHealth;
        IsAlive = true;
    }

    void Update()
    {
        if (!IsAlive) return;
        // 플레이어 입력 처리 및 이동 로직
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
