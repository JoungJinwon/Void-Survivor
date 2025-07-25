using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefaultSettingsUI : MonoBehaviour
{
    [Header("Audio UI")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Audio Labels")]
    public TextMeshProUGUI masterVolumeLabel;
    public TextMeshProUGUI bgmVolumeLabel;
    public TextMeshProUGUI sfxVolumeLabel;
    
    [Header("Display UI")]
    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessLabel;
    
    [Header("Buttons")]
    public Button resetButton;
    
    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }
    
    private void InitializeUI()
    {
        if (SettingsManager.Instance == null || SettingsManager.Instance.playerSettings == null)
        {
            Debug.LogError("SettingsManager or PlayerSettings not found!");
            return;
        }
        
        var settings = SettingsManager.Instance.playerSettings;
        
        // 슬라이더 값 설정
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = settings.masterVolume;
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = settings.musicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = settings.sfxVolume;
        if (brightnessSlider != null)
            brightnessSlider.value = settings.screenBrightness;
        
        // 라벨 업데이트
        UpdateLabels();
    }
    
    private void SetupEventListeners()
    {
        // 슬라이더 이벤트
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        
        // 버튼 이벤트
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMasterVolume(value);
        UpdateLabels();
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
        UpdateLabels();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
        UpdateLabels();
    }
    
    private void OnBrightnessChanged(float value)
    {
        SettingsManager.Instance.SetScreenBrightness(value);
        UpdateLabels();
    }
    
    private void OnResetButtonClicked()
    {
        SettingsManager.Instance.ResetToDefaults();
        InitializeUI(); // UI 다시 초기화
    }
    
    private void UpdateLabels()
    {
        if (SettingsManager.Instance == null || SettingsManager.Instance.playerSettings == null)
            return;
        
        var settings = SettingsManager.Instance.playerSettings;
        
        if (masterVolumeLabel != null)
            masterVolumeLabel.text = $"{(settings.masterVolume * 100):F0}";
        if (bgmVolumeLabel != null)
            bgmVolumeLabel.text = $"{(settings.musicVolume * 100):F0}";
        if (sfxVolumeLabel != null)
            sfxVolumeLabel.text = $"{(settings.sfxVolume * 100):F0}";
        if (brightnessLabel != null)
            brightnessLabel.text = $"{(settings.screenBrightness * 100):F0}";
    }
    
    private void OnDestroy()
    {
        // 이벤트 해제
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(OnBrightnessChanged);
        
        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetButtonClicked);
    }
}
