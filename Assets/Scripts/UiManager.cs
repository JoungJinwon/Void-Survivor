using System.Collections;
using System.Collections.Generic;
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

    [Header("Player Setting UI")]
    public GameObject weaponPanel;
    public Animator weaponPanelAnimator;
    [Space(10)]

    [Header("Survival UI")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expBarText;
    public Slider expBar;

    public Canvas skillCanvas;
    public GameObject[] skillButtons;
    public Image[] skillIcons;
    public TextMeshProUGUI[] skillHeaderTexts;
    public TextMeshProUGUI[] skillDecriptionTexts;

    public GameObject pausePanel;
    public GameObject equippedSkillGrid;
    public GameObject skillSlotPrefab;

    public TextMeshProUGUI playerStatsText; // 디버깅용 플레이어 스탯 텍스트

    public GameObject gameOverCanvas;
    public GameObject gameOverPanel;
    public TextMeshProUGUI stageRecordText;
    public TextMeshProUGUI timeRecordText;

    [Header("SFX")]
    public AudioClip SkillPopSound;
    [Space(10)]

    public GameManager _GM;

    private Queue<int> pendingLevelUps = new Queue<int>();
    private bool isProcessingLevelUp = false;

    private AudioSource _AudioSource;

    #region Unity Event Methods
    private void Awake()
    {
        InitSingleton();

        if (GameManager.Instance != null)
            _GM = GameManager.Instance;
        else
            Debug.LogWarning("UI Manager: GM 어딨어?");

        _AudioSource = GetComponent<AudioSource>();

        Debug.Log("UiManager Awake Ready");
    }

    public void Start()
    {
        if (skillSlotPrefab == null)
        {
            skillSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Skill Slot");
        }

        if (SkillPopSound == null)
        {
            SkillPopSound = Resources.Load<AudioClip>("Audio/SFX/skill_PopUp_Sound");
        }
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
                if (_GM != null && !_GM.IsGamePaused)
                    UpdateGameTimeText(_GM.GameTime);
                UpdatePlayerStatsText();
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
        if (fadeCanvas == null)
            fadeCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Fade Canvas"));

        fadePanel = fadeCanvas.transform.Find("Fade Panel").gameObject;
        if (fadePanel != null)
            _fadeAnimator = fadePanel.GetComponent<Animator>();
        else
            Debug.LogWarning($"Ui Manager: {fadePanel}을 찾을 수 없습니다");
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

    #region Player Setting UI
    public void ActiveWindowSlide()
    {
        if (weaponPanelAnimator != null)
        {
            if (weaponPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("WindowSlide_Active"))
            {
                weaponPanelAnimator.SetTrigger("Inactive");
            }
            else if (weaponPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("WindowSlide_Inactive"))
            {
                weaponPanelAnimator.SetTrigger("Active");
            }
        }
    }
    #endregion

    #region Survival UI
    public void UpdatePhaseText(string phaseName)
    {
        phaseText.text = $"{phaseName}";
    }

    public void UpdateGameTimeText(float gameTime)
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        Debug.Log($"Game Time: {minutes:D2}:{seconds:D2}");
        gameTimeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void UpdatePlayerStatsText()
    {
        if (playerStatsText != null)
        {
            playerStatsText.text = $"Attack: {_GM._Player.GetPlayerAttackDamage()}\n" +
                                   $"Attack Speed: {_GM._Player.GetPlayerAttackSpeed()}\n" +
                                   $"Move Speed: {_GM._Player.GetPlayerMoveSpeed()}\n" +
                                   $"Weapon: {_GM._Player.GetPlayerWeapon()?.name ?? "None"}";
        }
        else
        {
            Debug.LogWarning("Player Stats Text is not assigned in UiManager.");
        }
    }

    public void HandleLevelUp(int newLevel)
    {
        pendingLevelUps.Enqueue(newLevel);

        if (!isProcessingLevelUp)
        {
            StartCoroutine(ProcessLevelUps());
        }
    }

    private IEnumerator ProcessLevelUps()
    {
        isProcessingLevelUp = true;

        // 게임 일시정지 (첫 번째 레벨업에서만)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }

        while (pendingLevelUps.Count > 0)
        {
            int level = pendingLevelUps.Dequeue();

            UpdateLevelText(level);
            ActivateSkillCanvas();

            // 스킬 선택을 기다림
            yield return new WaitUntil(() => !skillCanvas.gameObject.activeSelf);

            // 다음 레벨업이 있다면 잠시 대기
            if (pendingLevelUps.Count > 0)
                yield return new WaitForSeconds(0.5f);
        }

        isProcessingLevelUp = false;

        GameManager.Instance.ResumeGame();
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = $"LEVEL {level}";
    }

    public void UpdateExpBar(float exp, float maxExp)
    {
        expBarText.text = $"{(int)exp}/{(int)maxExp}";
        expBar.value = exp / maxExp;
    }

    public void ActivateSkillCanvas()
    {
        skillCanvas.gameObject.SetActive(true);

        List<Skill> skills = SkillManager.Instance.GetNewSkills();

        if (skills.Count > 0)
        {
            // IndexOutOfRange 에러 방지를 위한 안전 검사
            int maxIndex = Mathf.Min(skills.Count, skillButtons.Length);

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < skillIcons.Length && i < skillHeaderTexts.Length && i < skillDecriptionTexts.Length)
                {
                    skillIcons[i].sprite = skills[i].icon;
                    skillHeaderTexts[i].text = skills[i].skillName;
                    skillDecriptionTexts[i].text = skills[i].skillDescription;

                    Debug.Log($"{i}번째 스킬: {skillHeaderTexts[i].text}");
                }
            }
        }

        float skillButtonOffset = 200f;
        switch (skills.Count)
        {
            case 1:
                skillButtons[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                skillButtons[1].SetActive(false);
                skillButtons[2].SetActive(false);
                break;
            case 2:
                skillButtons[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-skillButtonOffset, 0);
                skillButtons[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(skillButtonOffset, 0);
                skillButtons[2].SetActive(false);
                break;
            default:
                break;
        }
    }

    public void DeactivateSkillCanvas()
    {
        skillCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// 스킬 선택 완료 후 호출되는 메서드
    /// </summary>
    public void OnSkillSelected()
    {
        // 스킬 캔버스 비활성화
        DeactivateSkillCanvas();

        // 다음 레벨업이 있는지 확인하고, 없다면 게임 재개는 ProcessLevelUps에서 처리
        Debug.Log("Skill selected");
    }

    public void UpdateEquippedSkillsGrid(Skill skill)
    {
        if (equippedSkillGrid == null)
        {
            Debug.LogWarning("Ui Manager: Equipped Skill Grid or Skill Slot Prefab is not assigned.");
            return;
        }

        GameObject skillSlot = Instantiate(skillSlotPrefab, equippedSkillGrid.transform);
        Image skillImage = skillSlot.transform.GetChild(0).GetComponent<Image>();
        if (skillImage != null)
            skillImage.sprite = skill.icon;
        else
            Debug.LogWarning($"Ui Manager: Could not find child Image component in skill slot for {skill.skillName}");
    }

    /// <summary>
    /// 일시정지 UI 표시 (UI 버튼 클릭 시)
    /// </summary>
    public void ActivatePausePanel()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause Panel is NULL");
            return;
        }

        pausePanel.SetActive(true);
    }

    /// <summary>
    /// 일시정지 UI 숨김 (게임 재개 시)
    /// </summary>
    public void DeactivatePausePanel()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause Panel is NULL");
            return;
        }

        pausePanel.SetActive(false);
    }

    /// <summary>
    /// UI 일시정지 버튼 클릭 시 호출
    /// </summary>
    public void OnPauseButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }

        ActivatePausePanel();
    }

    /// <summary>
    /// UI 재개 버튼 클릭 시 호출
    /// </summary>
    public void OnResumeButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }

        DeactivatePausePanel();
    }

    public void ActivateGameOverPanel()
    {
        if (gameOverCanvas == null)
        {
            Debug.LogWarning("Game Over Canvas is NULL");
            return;
        }

        gameOverCanvas.SetActive(true);
        StartCoroutine(ActivateGameOverPanelCoroutine());
    }

    private IEnumerator ActivateGameOverPanelCoroutine()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        stageRecordText.text = $"{PhaseManager.Instance.GetCurrentPhaseName()}";
        timeRecordText.text = $"Time record:\n{gameTimeText.text}";

        yield return new WaitForSeconds(0.8f); // 게임 오버 패널 활성화 후 잠시 대기

        gameOverPanel.SetActive(true);
    }
    #endregion

}