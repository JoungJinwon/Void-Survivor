using System.Collections;
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

    private void Awake()
    {
        InitSingleton();
        InitGameManager();

        Debug.Log("GameManager Awake Ready");
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

        SceneManager.sceneLoaded += OnSceneLoaded;

        _Player = FindFirstObjectByType<Player>();
        if (_Player == null)
            Debug.LogWarning("Game Manager: Player를 찾지 못했습니다");
        else
            Debug.Log("Game Manager: Player를 성공적으로 찾았습니다.");
    }

    private void InitSurvivalScene()
    {
        if (PhaseManager.Instance != null)
            _PM = PhaseManager.Instance;
            
        GameTime = 0f;
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

    /// <summary>
    /// 플레이어 레벨업 시 게임을 일시정지합니다 (스킬 선택용)
    /// </summary>
    public void PauseForLevelUp()
    {
        PauseGame();
        Debug.Log("Game Paused for Level Up - Waiting for skill selection");
    }

    /// <summary>
    /// 스킬 선택 완료 후 게임을 재개합니다
    /// </summary>
    public void ResumeFromLevelUp()
    {
        ResumeGame();
        Debug.Log("Game Resumed from Level Up - Skill selected");
    }

    public void GameExit()
    {
        Debug.Log("Game Exiting...");
        Application.Quit();
    }
#endregion

}
