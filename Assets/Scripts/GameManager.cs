using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneData;

/// <summary>
/// 게임, 씬, 매니저들을 관리하는 최상위 관리자 클래스입니다.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public bool IsGamePaused { get; private set; } = false;
    public float GameTime { get; private set; } = 0f; // 게임 시간(seconds)

    public CurrentScene currentScene = CurrentScene.Main;

    public Player _Player { get; private set; }

    public UiManager _UM { get; private set; }
    public PhaseManager _PM { get; private set; }
    public SettingsManager _SM { get; private set; }

    private void Awake()
    {
        InitSingleton();
        InitGameManager();

        // 애플리케이션 종료 시 스킬 리셋
        Application.quitting += OnApplicationQuitting;
    }
    
    private void OnApplicationQuitting()
    {
        Debug.Log("Application is quitting - Resetting skills to initial values");
        ResetAllSkillsToInitialValues();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // 이벤트 해제
        Application.quitting -= OnApplicationQuitting;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (currentScene == CurrentScene.Survival && !IsGamePaused)
        {
            GameTime += Time.deltaTime;

            if (_PM != null)
                _PM.UpdatePhase(GameTime);
        }
    }

    #region Initialization
    private void InitGameManager()
    {
        if (UiManager.Instance != null)
            _UM = UiManager.Instance;

        if (SettingsManager.Instance != null)
            _SM = SettingsManager.Instance;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
#endregion

#region Scene Management
    /// <summary>
    /// Load Scene by INDEX
    /// </summary>
    public void LoadSceneWithIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Game Manager: Invalid Scene Index");
            return;
        }
        else
            StartCoroutine(LoadSceneWithIndexAsync(sceneIndex));
    }

    /// <summary>
    /// Load Scene by NAME
    /// </summary>
    public void LoadSceneWithName(string sceneName)
    {
        int sceneIndex = SceneManager.GetSceneByName(sceneName).buildIndex;

        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Game Manager: {sceneIndex} is Invalid Scene Index");
            return;
        }
        else
            StartCoroutine(LoadSceneWithIndexAsync(sceneIndex));
    }

    /// <summary>
    /// Coroutine which loads a scene asynchronously
    /// </summary>
    public IEnumerator LoadSceneWithIndexAsync(int sceneIndex)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);
        loadOp.allowSceneActivation = false;

        yield return StartCoroutine(UiManager.Instance.FadeOut());

        while (!loadOp.isDone)
        {
            Debug.Log("Game Manager: Loading Scene...(Operation is not done yet)");

            if (loadOp.progress >= 0.9f && _UM.IsFading == false)
            {
                yield return new WaitForSeconds(0.5f);
                loadOp.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log($"Game Manager: Scene {sceneIndex} Loaded");
        loadOp.allowSceneActivation = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = (CurrentScene)scene.buildIndex;

        switch (currentScene)
        {
            case CurrentScene.Main:
                UiManager.Instance.InitUiManager_Main();
                break;
            case CurrentScene.PlayerSetting:
                UiManager.Instance.InitUiManager_PlayerSettings();
                // 설정 씬 로드 시 설정값 적용
                if (SettingsManager.Instance != null)
                    SettingsManager.Instance.ApplySettings();
                break;
            case CurrentScene.Survival:
                InitSurvivalScene();
                UiManager.Instance.InitUiManager_Survival();
                break;
            default:
                Debug.LogError($"Game Manager: {currentScene} is Invalid Scene");
                break;
        }
    }
    
    private void InitSurvivalScene()
    {
        // 게임 상태 초기화
        GameTime = 0f;
        IsGamePaused = false;
        
        // Time.timeScale 정상화 (게임오버 시 변경될 수 있음)
        Time.timeScale = 1f;
        
        _Player = FindFirstObjectByType<Player>();
        if (_Player == null)
            Debug.LogWarning("Game Manager: Player를 찾지 못했습니다");
        else
            Debug.Log("Game Manager: Player를 성공적으로 찾았습니다.");

        if (PhaseManager.Instance != null)
        {
            _PM = PhaseManager.Instance;
            // PhaseManager 재시작 시 리셋
            _PM.ResetPhaseManager();
        }
            
        // 모든 기존 적들을 제거 (재시작 시)
        CleanupExistingEnemies();
        
        Debug.Log("Survival Scene Initialized Successfully");
    }
    
    /// <summary>
    /// 씬에 남아있는 기존 적들을 정리합니다
    /// </summary>
    private void CleanupExistingEnemies()
    {
        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in existingEnemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        
        if (existingEnemies.Length > 0)
            Debug.Log($"Cleaned up {existingEnemies.Length} existing enemies");
    }
#endregion

    #region Game Control
    /// <summary>
    /// 게임을 일시정지합니다
    /// </summary>
    public void PauseGame()
    {
        if (IsGamePaused) return;

        IsGamePaused = true;
        Debug.Log("Game Paused");
    }

    /// <summary>
    /// 게임을 재개합니다
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGamePaused) return;

        IsGamePaused = false;
        Debug.Log("Game Resumed");
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        // 게임 오버 UI 활성화
        if (_UM != null)
            _UM.ActivateGameOverPanel();

        // 게임 오버 코루틴 시작
        StartCoroutine(GameOVerCoroutine());
    }

    private IEnumerator GameOVerCoroutine()
    {
        IsGamePaused = true;

        Time.timeScale = 0.5f;

        yield return new WaitForSecondsRealtime(1f); // 1초 동안 게임 속도 감소

        Time.timeScale = 1f; // 게임 속도 원래대로
    }

    // 서바이벌씬에서 나오는 함수
    public void ExitSurvival()
    {
        ResetAllSkillsToInitialValues();
    }

    /// <summary>
    /// 서바이벌 씬을 재시작합니다
    /// </summary>
    public void RestartSurvival()
    {
        Debug.Log("Restarting Survival Scene...");
        
        // 모든 스킬을 초기값으로 리셋
        ResetAllSkillsToInitialValues();
        
        // 게임 상태 초기화
        IsGamePaused = false;
        GameTime = 0f;
        
        // Singleton 인스턴스들 재설정 (씬 재로드 대비)
        ResetSingletonInstances();
        
        // 현재 씬을 다시 로드
        LoadSceneWithIndex((int)CurrentScene.Survival);
    }

    /// <summary>
    /// 모든 Singleton 매니저들의 인스턴스를 재설정합니다
    /// </summary>
    private void ResetSingletonInstances()
    {
        // 주의: GameManager는 DontDestroyOnLoad 오브젝트이므로 재설정하지 않음
        // 대신 각 매니저들이 씬 로드 시 자동으로 재초기화되도록 함
        Debug.Log("Singleton instances will be reset during scene reload");
    }

    // 게임 자체를 종료하는 함수
    public void ExitGame()
    {
        Debug.Log("Game Exiting...");

        // 설정 저장
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SaveSettings();

        if (currentScene == CurrentScene.Main)
        {
            
        }
        else if (currentScene == CurrentScene.Survival)
        {
            // 게임 종료 시 모든 스킬을 초기값으로 리셋
            ResetAllSkillsToInitialValues();
        }

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 종료
    #else
        Application.Quit();
    #endif
    }
    
    /// <summary>
    /// 모든 스킬을 초기값으로 리셋합니다
    /// </summary>
    private void ResetAllSkillsToInitialValues()
    {
        if (SkillManager.Instance != null && SkillManager.Instance.skillDatabase != null)
        {
            int resetCount = 0;
            foreach (Skill skill in SkillManager.Instance.skillDatabase)
            {
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
                        resetCount++;
                    }
                }
            }
            
            if (resetCount > 0)
            {
                Debug.Log($"All {resetCount} skills have been reset to their initial values.");
            }
            else
            {
                Debug.Log("No skills found with stored initial values to reset.");
            }
        }
    }
#endregion
}
