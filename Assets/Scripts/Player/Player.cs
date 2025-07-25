using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{   
    // 플레이어 기본 스탯 (플레이 중 이 값들은 변경되며, 플레이어 base Stat에는 영향을 미치지 않는다)
    [SerializeField] private float maxHealth;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float moveSpeed;

    private int level = 1;
    private int maxLevel = 20;
    private float exp;
    private float maxExp = 100f;
    private float currentHealth;

    [SerializeField] private Slider hpBar; // Player의 체력바를 표시할 Slider UI
    [SerializeField] private Slider expBar; // Player의 경험치를 표시할 Slider UI

    private Weapon _weapon; // 장착된 Weapon
    private AudioSource _audioSource;

    public Enemy targetEnemy;

    private void Start()
    {
        InitPlayer();
    }

    private void Update()
    {
        if (!IsAlive || GameManager.Instance.IsGamePaused) return;
        // 플레이어 입력 처리 및 이동 로직

        // 무기 자동 사용
        if (_weapon != null)
        {
            _weapon.TryAttack(this); // 플레이어 공격 속도 전달
            Debug.Log($"Player: {_weapon} 공격!");
        }
        else
        {
            Debug.LogWarning("Player: 장착된 Weapon이 존재하지 않습니다");
        }
    }

    private void InitPlayer()
    {
        // 현재 스탯을 기본 스탯으로 초기화
        ResetStatsToBase();
        
        currentHealth = maxHealth;
        IsAlive = true;

        _weapon = SettingsManager.Instance.playerSettings.playerWeapon;
        _weapon?.InitWeapon();

        _audioSource = GetComponent<AudioSource>();
        if (_weapon?.attackSound != null)
            _audioSource.clip = _weapon.attackSound;

        UiManager.Instance.UpdateLevelText(level);
        UiManager.Instance.UpdateExpBar(exp, maxExp);
    }
    
    private void ResetStatsToBase()
    {
        maxHealth = SettingsManager.Instance.GetBaseMaxHealth();
        attackDamage = SettingsManager.Instance.GetBaseAttackDamage();
        attackSpeed = SettingsManager.Instance.GetBaseAttackSpeed();
        moveSpeed = SettingsManager.Instance.GetBaseMoveSpeed();
    }
    
    // 게임 종료 시 기본 스탯을 현재 스탯으로 업데이트하고 저장 (선택적)
    public void SaveCurrentStatsAsBase()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetBaseMaxHealth(maxHealth);
            SettingsManager.Instance.SetBaseAttackDamage(attackDamage);
            SettingsManager.Instance.SetBaseAttackSpeed(attackSpeed);
            SettingsManager.Instance.SetBaseMoveSpeed(moveSpeed);
            
            Debug.Log("Current stats saved as new base stats");
        }
    }

    public void PlayAttackSound()
    {
        if (_audioSource != null && _weapon?.attackSound != null)
        {
            _audioSource.PlayOneShot(_weapon.attackSound);
        }
        else
        {
            Debug.LogWarning("Player: AudioSource or attack sound is not set!");
        }
    }

    #region Getters

    public Player GetPlayer() { return this; }
    public Weapon GetPlayerWeapon() { return _weapon; }
    public float GetPlayerMaxHealth() { return maxHealth; }
    public int GetPlayerAttackDamage() { return attackDamage; }
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
        
        // Handle multiple level ups sequentially
        while (exp >= maxExp && level < maxLevel)
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

            IncreaseHealth(10f);
            IncreaseAttack(1);
            IncreaseAttackSpeed(1.05f);
            IncreaseMoveSpeed(1.05f);
            maxExp = CalculateMaxExp(level);

            // 게임 일시정지 및 UI 업데이트를 UiManager에서 처리
            if (UiManager.Instance != null)
            {
                UiManager.Instance.HandleLevelUp(level);
            }
        }
    }

    private float CalculateMaxExp(int level)
    {
        return 100f * Mathf.Pow(1.5f, level - 1);
    }

    // 플레이어 체력 증가 메서드. 체력 증가 시 기존 체력 비율 유지.
    // 레벨업 및 스킬에서 사용된다 (임시 증가)
    public void IncreaseHealth(float amount)
    {
        float curHpRate = currentHealth / maxHealth;
        maxHealth += amount;
        currentHealth = maxHealth * curHpRate;
        UpdateHealthBar();
    }

    public void IncreaseAttack(int amount)
    {
        attackDamage += amount;
    }

    public void DecreaseAttack(int amount)
    {
        attackDamage = Mathf.Max(10, attackDamage - amount);
    }

    public void IncreaseAttackSpeed(float multiplier)
    {
        attackSpeed *= multiplier;
    }

    public void DecreaseAttackSpeed(float multiplier)
    {
        attackSpeed = Mathf.Max(0.1f, attackSpeed / multiplier);
    }

    public void IncreaseMoveSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    public void DecreaseMoveSpeed(float multiplier)
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed / multiplier);
    }

    public void Accelerate(float attackSpeedBoost, float moveSpeedBoost)
    {
        IncreaseAttackSpeed(attackSpeedBoost);
        IncreaseMoveSpeed(moveSpeedBoost);

        Debug.Log($"Player: (공격력/공격 속도/이동 속도) = {attackDamage}/{attackSpeed}/{moveSpeed}");
    }

    public void Decelerate(float attackSpeedBoost, float moveSpeedBoost)
    {
        DecreaseAttackSpeed(attackSpeedBoost);
        DecreaseMoveSpeed(moveSpeedBoost);

        Debug.Log($"Player: (공격력/공격 속도/이동 속도) = {attackDamage}/{attackSpeed}/{moveSpeed}");
    }

    #endregion

    private void UpdateHealthBar()
    {
        if (hpBar == null) return;

        hpBar.value = currentHealth / maxHealth;
    }
}
