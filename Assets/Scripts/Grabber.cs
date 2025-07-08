using TMPro;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public GameManager _GM;
    public UiManager _UM;

    [Header("Survival UI")]
    public TextMeshProUGUI phaseText;

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
    }
#endregion

#region Game Manager
    public void LoadScene_G(int sceneIdx)
    {
        _GM.LoadSceneWithIndex(sceneIdx);
    }
#endregion

#region UI Manager
    public void AllocateSurvivalUi()
    {
        _UM.phaseText = phaseText;
    }
#endregion
}