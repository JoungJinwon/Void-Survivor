// #if UNITY_EDITOR

// using UnityEngine;
// using UnityEditor;

// [InitializeOnLoad]
// public static class EditorResetter
// {
//     static EditorResetter()
//     {
//         EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//     }

//     private static void OnPlayModeStateChanged(PlayModeStateChange state)
//     {
//         GameObject fadeCanvas = GameObject.Find("Fade Canvas")?.gameObject;

//         if (fadeCanvas == null)
//         {
//             Debug.LogWarning("Fade Canvas not found in the scene. Please ensure it exists.");
//             return;
//         }
        
//         if (state == PlayModeStateChange.ExitingPlayMode) // 플레이 모드 종료 직전
//             {
//                 if (fadeCanvas != null)
//                 {
//                     fadeCanvas.SetActive(false);
//                 }
//             }
//             else if (state == PlayModeStateChange.EnteredPlayMode) // 플레이 모드 시작 직후
//             {
//                 if (fadeCanvas != null)
//                 {
//                     fadeCanvas.SetActive(true);
//                 }
//             }
//     }
// }
// #endif