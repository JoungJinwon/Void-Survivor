#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class EditorResetter
{
    static EditorResetter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode) // 플레이 모드 종료 직전
        {
            Debug.Log("Editor: Exiting play mode - Resetting values...");
            ResetWeaponProjectileCounts();
            ResetAllSkillsToInitialValues();
        }
        else if (state == PlayModeStateChange.EnteredEditMode) // 에디터 모드 진입 후
        {
            Debug.Log("Editor: Entered edit mode - Ensuring all assets are saved...");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static void ResetWeaponProjectileCounts()
    {
        // 모든 Weapon ScriptableObject를 찾아서 투사체 개수를 초기화
        string[] weaponGUIDs = AssetDatabase.FindAssets("t:Weapon");
        
        foreach (string guid in weaponGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Weapon weapon = AssetDatabase.LoadAssetAtPath<Weapon>(path);
            
            if (weapon != null)
            {
                if (weapon is SpinnerWeapon)
                    weapon.projectileCount = 2;
                else
                    weapon.projectileCount = 1; // 기본값으로 초기화
                EditorUtility.SetDirty(weapon);
            }
        }
        
        AssetDatabase.SaveAssets();
    }
    
    private static void ResetAllSkillsToInitialValues()
    {
        // 모든 Skill ScriptableObject를 찾아서 초기값으로 리셋
        string[] skillGUIDs = AssetDatabase.FindAssets("t:Skill");
        int resetCount = 0;
        
        foreach (string guid in skillGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Skill skill = AssetDatabase.LoadAssetAtPath<Skill>(path);
            
            if (skill != null)
            {
                // 초기값이 저장되지 않았다면 먼저 저장
                if (!skill.hasStoredInitialValues)
                {
                    skill.StoreInitialValues();
                }
                else
                {
                    skill.ResetToInitialValues();
                    EditorUtility.SetDirty(skill);
                    resetCount++;
                }
            }
        }
        
        if (resetCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"All {resetCount} skills have been reset to their initial values.");
        }
        else
        {
            Debug.Log("No skills found with stored initial values to reset.");
        }
    }
    
    [MenuItem("Tools/Reset All Skills to Initial Values")]
    private static void ManualResetAllSkills()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Cannot reset skills while in play mode. Stop play mode first.");
            return;
        }
        
        ResetAllSkillsToInitialValues();
    }
}
#endif