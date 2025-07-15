using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Grabber : MonoBehaviour
{
    public GameManager _GM;
    public UiManager _UM;

    [Header("Player Setting UI")]
    public GameObject weaponPanel;

    [Header("Survival UI")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expBarText;
    public Slider expBar;

    public Canvas skillCanvas;
    public Image[] skillIconImages;
    public TextMeshProUGUI[] skillHeaderTexts;
    public TextMeshProUGUI[] skillDecriptionTexts;

    public GameObject pausePanel;
    public GameObject equippedSkillGrid;

    private void Awake()
    {
        InitGrabber();
    }

    #region  Initialization
    private void InitGrabber()
    {
        if (_GM == null && GameManager.Instance != null)
            _GM = GameManager.Instance;
        if (_UM == null && UiManager.Instance != null)
        {
            _UM = UiManager.Instance;
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            AllocatePlayerSettingUi();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            AllocateSurvivalUi();
        }
    }
    #endregion

    #region Game Manager
    public void LoadScene_G(int sceneIdx)
    {
        _GM.LoadSceneWithIndex(sceneIdx);
    }
    #endregion

    #region UI Manager
    public void AllocatePlayerSettingUi()
    {
        _UM.weaponPanel = weaponPanel;
        _UM.weaponPanelAnimator = weaponPanel.GetComponent<Animator>();

        Debug.Log("Grabber: Player Setting UI 할당 완료");
    }

    public void AllocateSurvivalUi()
    {
        _UM.phaseText = phaseText;
        _UM.levelText = levelText;
        _UM.expBarText = expBarText;
        _UM.expBar = expBar;
        _UM.skillCanvas = skillCanvas;
        _UM.skillIcons = skillIconImages;
        _UM.skillHeaderTexts = skillHeaderTexts;
        _UM.skillDecriptionTexts = skillDecriptionTexts;
        _UM.pausePanel = pausePanel;
        _UM.equippedSkillGrid = equippedSkillGrid;
    }

    public void ActiveWindowSlide()
    {
        _UM.ActiveWindowSlide();
    }

    public void OnPauseButtonClicked()
    {
        _UM.OnPauseButtonClicked();
    }

    public void OnResumeButtonClicked()
    {
        _UM.OnResumeButtonClicked();
    }
    #endregion
}