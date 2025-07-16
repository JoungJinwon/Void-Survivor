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
            ResetWeaponProjectileCounts();
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
                Debug.Log($"Reset projectile count for {weapon.name}");
            }
        }
        
        AssetDatabase.SaveAssets();
    }
}
#endif