using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float moveSpeed = 1f;

    private int level = 1;
    private int maxLevel = 20;
    private float exp;
    private float maxExp = 100f;
    private float currentHealth;

    [SerializeField] private Slider hpBar; // Player의 체력바를 표시할 Slider UI
    [SerializeField] private Slider expBar; // Player의 경험치를 표시할 Slider UI

    private Weapon weapon; // 장착된 Weapon

    private void Start()
    {
        InitPlayer();
    }

    private void Update()
    {
        if (!IsAlive || GameManager.Instance.IsGamePaused) return;
        // 플레이어 입력 처리 및 이동 로직

        // 무기 자동 사용
        if (weapon != null)
        {
            weapon.TryAttack(this); // 플레이어 공격 속도 전달
            Debug.Log($"Player: {weapon} 공격!");
        }
        else
        {
            Debug.LogWarning("Player: 장착된 Weapon이 존재하지 않습니다");
        }
    }

    private void InitPlayer()
    {
        currentHealth = maxHealth;
        IsAlive = true;

        weapon = GetComponent<WeaponSlot>()?.equippedWeapon;
        weapon?.InitWeapon();

        UiManager.Instance.UpdateLevelText(level);
        UiManager.Instance.UpdateExpBar(exp, maxExp);
    }

    #region Getters

    public Player GetPlayer() { return this; }
    public Weapon GetPlayerWeapon() { return weapon; }
    public float GetPlayerMaxHealth() { return maxHealth; }
    public float GetPlayerAttackDamage() { return attackDamage; }
    public float GetPlayerAttackSpeed() { return attackSpeed; }
    public float GetPlayerMoveSpeed() { return moveSpeed; }

    #endregion

    public override void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0 && IsAlive)
        {
            Die();
        }
    }

    protected override void Die()
    {
        IsAlive = false;
        // 게임 오버 처리
    }

    #region Update Status

    public void GainExp(float amount)
    {
        if (level == maxLevel) return;

        exp += amount;
        if (exp >= maxExp)
        {
            exp -= maxExp;
            LevelUp();
        }

        UiManager.Instance.UpdateExpBar(exp, maxExp);
    }

    private void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;
            currentHealth = maxHealth; // 레벨업 시 체력 회복

            IncreaseHealth(20f);
            IncreaseAttack(2f); // 레벨업 시 공격력 증가
            IncreaseAttackSpeed(1.1f); // 레벨업 시 공격 속도 증가
            maxExp = level * 100f;

            // 게임 일시정지 (스킬 선택을 위해)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseForLevelUp();
            }

            UiManager.Instance.LevelUpUi(level);
        }
    }

    // 플레이어 체력 증가 메서드. 체력 증가 시 기존 체력 비율 유지.
    // 레벨업 및 스킬에서 사용된다
    public void IncreaseHealth(float amount)
    {
        float curHpRate = currentHealth / maxHealth;
        maxHealth += amount;
        currentHealth = maxHealth * curHpRate;
        UpdateHealthBar();

        Debug.Log($"Player: (최대 체력/현재 체력) = {maxHealth}/{currentHealth}");
    }

    public void IncreaseAttack(float amount)
    {
        attackDamage += amount;
    }

    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeed *= amount;
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed *= amount;
    }

    #endregion

    private void UpdateHealthBar()
    {
        if (hpBar == null) return;

        hpBar.value = currentHealth / maxHealth;
    }
}
