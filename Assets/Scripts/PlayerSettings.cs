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
    
    public void ResetToDefaults()
    {
        masterVolume = 0.5f;
        musicVolume = 0.5f;
        sfxVolume = 0.5f;
        screenBrightness = 1f;
    }
}
