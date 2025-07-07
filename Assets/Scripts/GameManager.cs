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

    UiManager _UM;
    PhaseManager _PM;

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
        if (PhaseManager.Instance != null)
            _PM = PhaseManager.Instance;
            
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void InitSurvivalScene()
    {
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
                UiManager.Instance.InitUiManager_Survival();
                break;
            default:
                Debug.LogError($"Game Manager: {currentScene} is Invalid Scene");
                break;
        }
    }
#endregion

#region Game Control
    public void GameExit()
    {
        Debug.Log("Game Exiting...");
        Application.Quit();
    }
#endregion

}
