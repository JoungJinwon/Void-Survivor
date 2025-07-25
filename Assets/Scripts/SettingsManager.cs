using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : Singleton<SettingsManager>
{
    [Header("Settings")]
    public PlayerSettings playerSettings;
    
    [Header("Audio")]
    public AudioMixer audioMixer;
    
    [Header("Display")]
    public CanvasGroup screenOverlay; // UI 오버레이로 밝기 조절
    
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string BRIGHTNESS_KEY = "ScreenBrightness";
    
    // 플레이어 스탯 저장 키들
    private const string BASE_HEALTH_KEY = "BaseMaxHealth";
    private const string BASE_ATTACK_KEY = "BaseAttackDamage";
    private const string BASE_ATTACK_SPEED_KEY = "BaseAttackSpeed";
    private const string BASE_MOVE_SPEED_KEY = "BaseMoveSpeed";
    
    private void Awake()
    {
        InitSingleton();
        LoadSettings();
    }
    
    public void LoadSettings()
    {
        if (playerSettings == null)
        {
            playerSettings = Resources.Load<PlayerSettings>("Scriptable Objects/Player/PlayerSettings");
        }
        
        // PlayerPrefs에서 설정값 로드 (기본값은 현재 설정값)
        playerSettings.masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, playerSettings.masterVolume);
        playerSettings.musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, playerSettings.musicVolume);
        playerSettings.sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, playerSettings.sfxVolume);
        playerSettings.screenBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, playerSettings.screenBrightness);
        
        // 플레이어 기본 스탯 로드
        playerSettings.baseMaxHealth = PlayerPrefs.GetFloat(BASE_HEALTH_KEY, playerSettings.baseMaxHealth);
        playerSettings.baseAttackDamage = PlayerPrefs.GetInt(BASE_ATTACK_KEY, playerSettings.baseAttackDamage);
        playerSettings.baseAttackSpeed = PlayerPrefs.GetFloat(BASE_ATTACK_SPEED_KEY, playerSettings.baseAttackSpeed);
        playerSettings.baseMoveSpeed = PlayerPrefs.GetFloat(BASE_MOVE_SPEED_KEY, playerSettings.baseMoveSpeed);
        
        // 설정값 적용
        ApplySettings();
        
        Debug.Log("Settings loaded from PlayerPrefs");
    }
    
    public void SaveSettings()
    {
        if (playerSettings == null) return;
        
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, playerSettings.masterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, playerSettings.musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, playerSettings.sfxVolume);
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, playerSettings.screenBrightness);
        
        // 플레이어 기본 스탯 저장
        PlayerPrefs.SetFloat(BASE_HEALTH_KEY, playerSettings.baseMaxHealth);
        PlayerPrefs.SetInt(BASE_ATTACK_KEY, playerSettings.baseAttackDamage);
        PlayerPrefs.SetFloat(BASE_ATTACK_SPEED_KEY, playerSettings.baseAttackSpeed);
        PlayerPrefs.SetFloat(BASE_MOVE_SPEED_KEY, playerSettings.baseMoveSpeed);
        
        PlayerPrefs.Save();
        
        Debug.Log("Settings saved to PlayerPrefs");
    }
    
    public void ApplySettings()
    {
        if (playerSettings == null) return;
        
        // 오디오 설정 적용
        ApplyAudioSettings();
        
        // 화면 밝기 설정 적용
        ApplyDisplaySettings();
    }
    
    private void ApplyAudioSettings()
    {
        if (audioMixer != null)
        {
            // 볼륨을 데시벨로 변환 (0~1 범위를 -80~0 dB로 변환)
            float masterDB = playerSettings.masterVolume > 0 ? Mathf.Log10(playerSettings.masterVolume) * 20 : -80f;
            float musicDB = playerSettings.musicVolume > 0 ? Mathf.Log10(playerSettings.musicVolume) * 20 : -80f;
            float sfxDB = playerSettings.sfxVolume > 0 ? Mathf.Log10(playerSettings.sfxVolume) * 20 : -80f;
            
            audioMixer.SetFloat("MasterVolume", masterDB);
            audioMixer.SetFloat("BGMVolume", musicDB);
            audioMixer.SetFloat("SFXVolume", sfxDB);
        }
    }
    
    private void ApplyDisplaySettings()
    {
        if (screenOverlay != null)
        {
            // 밝기 조절 (1 - brightness로 어둡게 만들기)
            screenOverlay.alpha = playerSettings.screenBrightness;
        }
    }
    
    #region Audio Settings Management
    public void SetMasterVolume(float volume)
    {
        if (playerSettings == null) return;

        playerSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        SaveSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        if (playerSettings == null) return;
        
        playerSettings.musicVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        SaveSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        if (playerSettings == null) return;
        
        playerSettings.sfxVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        SaveSettings();
    }
    #endregion
    
    #region Display Settings Management
    public void SetScreenBrightness(float brightness)
    {
        if (playerSettings == null) return;

        playerSettings.screenBrightness = Mathf.Clamp(brightness, 0.1f, 2f);
        ApplyDisplaySettings();
        SaveSettings();
    }
    #endregion

    #region Player Settings Management
    public void SetPlayerWeapon(Weapon weapon)
    {
        if (playerSettings == null) return;

        playerSettings.playerWeapon = weapon;
        SaveSettings();

        Debug.Log($"Player weapon set to: {weapon.name}");
    }
    #endregion

    #region Player Stats Management
    public void SetBaseMaxHealth(float health)
    {
        if (playerSettings == null) return;
        
        playerSettings.SetBaseMaxHealth(health);
        SaveSettings();
        
        Debug.Log($"Base max health set to: {health}");
    }
    
    public void SetBaseAttackDamage(int damage)
    {
        if (playerSettings == null) return;
        
        playerSettings.SetBaseAttackDamage(damage);
        SaveSettings();
        
        Debug.Log($"Base attack damage set to: {damage}");
    }
    
    public void SetBaseAttackSpeed(float speed)
    {
        if (playerSettings == null) return;
        
        playerSettings.SetBaseAttackSpeed(speed);
        SaveSettings();
        
        Debug.Log($"Base attack speed set to: {speed}");
    }
    
    public void SetBaseMoveSpeed(float speed)
    {
        if (playerSettings == null) return;
        
        playerSettings.SetBaseMoveSpeed(speed);
        SaveSettings();
        
        Debug.Log($"Base move speed set to: {speed}");
    }
    
    // 기본 스탯 getter 메서드들
    public float GetBaseMaxHealth() => playerSettings?.baseMaxHealth ?? 100f;
    public int GetBaseAttackDamage() => playerSettings?.baseAttackDamage ?? 10;
    public float GetBaseAttackSpeed() => playerSettings?.baseAttackSpeed ?? 1f;
    public float GetBaseMoveSpeed() => playerSettings?.baseMoveSpeed ?? 1f;
    #endregion

    public void ResetToDefaults()
    {
        if (playerSettings == null) return;

        playerSettings.ResetToDefaults();
        ApplySettings();
        SaveSettings();

        Debug.Log("Settings reset to defaults");
    }
}
