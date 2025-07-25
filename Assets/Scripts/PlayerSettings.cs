using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Game/Player Settings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;

    [Header("Display Settings")]
    [Range(0.1f, 1f)]
    public float screenBrightness = 1f;

    [Header("Weapon Settings")]
    public Weapon playerWeapon;

    [Header("Player Base Stats")]
    public float baseMaxHealth = 100f;
    public int baseAttackDamage = 10;
    public float baseAttackSpeed = 1f;
    public float baseMoveSpeed = 1f;
    
    public void ResetToDefaults()
    {
        masterVolume = 0.5f;
        musicVolume = 0.5f;
        sfxVolume = 0.5f;
        screenBrightness = 1f;
        
        // 기본 스탯 초기화
        baseMaxHealth = 100f;
        baseAttackDamage = 10;
        baseAttackSpeed = 1f;
        baseMoveSpeed = 1f;
    }
    
    // 기본 스탯 설정 메서드들
    public void SetBaseMaxHealth(float health)
    {
        baseMaxHealth = Mathf.Max(1f, health);
    }
    
    public void SetBaseAttackDamage(int damage)
    {
        baseAttackDamage = Mathf.Max(1, damage);
    }
    
    public void SetBaseAttackSpeed(float speed)
    {
        baseAttackSpeed = Mathf.Max(0.1f, speed);
    }
    
    public void SetBaseMoveSpeed(float speed)
    {
        baseMoveSpeed = Mathf.Max(0.1f, speed);
    }
}
