using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    public bool IsFading { get; private set; }
    public bool InitComplete { get; private set; }

    [SerializeField]
    [Tooltip("Canvas which is used for Fading Effect")]
    private GameObject fadeCanvas;

    [Header("Fade Effect")]
    public GameObject fadePanel;
    private Animator _fadeAnimator;
    [Space(10)]

    [Header("Survival UI")]
    public TextMeshProUGUI phaseText;

    public GameManager _GM;

    #region Unity Event Methods
    private void Awake()
    {
        InitSingleton();

        if (GameManager.Instance != null)
            _GM = GameManager.Instance;
        else
            Debug.LogWarning("UI Manager: GM 어딨어?");

        // SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("UiManager Awake Ready");
    }

    public void Start()
    {
    }

    private void Update()
    {
        switch (_GM.currentScene)
        {
            case SceneData.CurrentScene.Main:
                break;
            case SceneData.CurrentScene.PlayerSetting:
                break;
            case SceneData.CurrentScene.Survival:
                break;
            default:
                Debug.LogError($"UI Manager: {_GM.currentScene} is Invalid Scene");
                break;
        }
    }
    #endregion

    #region Initialization
    public void InitUiManager_Main()
    {
        FindFadeCanvas();

        InitComplete = true;
        Debug.Log($"Ui Manager: Main 씬 초기화 완료");
    }

    public void InitUiManager_PlayerSettings()
    {
        FindFadeCanvas();

        InitComplete = true;
        Debug.Log($"Ui Manager: PlayerSetting 씬 초기화 완료");
    }

    public void InitUiManager_Survival()
    {
        FindFadeCanvas();

        InitComplete = true;
        Debug.Log($"Ui Manager: Survival 씬 초기화 완료");
    }

    // 씬이 로드될 때마다 호출되는 메소드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
    #endregion


    #region Fade Effect
    public void FindFadeCanvas()
    {
        fadeCanvas = GameObject.Find("Fade Canvas");
        if (fadeCanvas != null)
        {
            fadePanel = fadeCanvas.transform.Find("Fade Panel").gameObject;
            if (fadePanel != null)
                _fadeAnimator = fadePanel.GetComponent<Animator>();
            else
                Debug.LogWarning($"Ui Manager: {fadePanel}을 찾을 수 없습니다");
        }
        else
            Debug.LogWarning($"UI Manager: {fadeCanvas}를 찾을 수 없습니다.");
    }

    // 씬 진입 시 Fade In 효과
    public IEnumerator FadeIn()
    {
        IsFading = true;

        if (!fadePanel.activeSelf)
            fadePanel.SetActive(true);
        _fadeAnimator.SetTrigger("FadeIn");

        Debug.Log("UI Manager: Fade In . . .");
        yield return new WaitUntil(() => _fadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        IsFading = false;
        Debug.Log("UI Manager: Fade In Complete");
    }

    // 씬 전환 시 Fade Out 효과
    public IEnumerator FadeOut()
    {
        IsFading = true;

        if (!fadePanel.activeSelf)
            fadePanel.SetActive(true);
        _fadeAnimator.SetTrigger("FadeOut");

        Debug.Log("UI Manager: Fade Out . . .");
        yield return new WaitUntil(() => _fadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        IsFading = false;
        Debug.Log("UI Manager: Fade Out Complete");
    }
    #endregion

    #region Survival UI
    public void UpdatePhaseText(string phaseName)
    {

    }
    #endregion

}