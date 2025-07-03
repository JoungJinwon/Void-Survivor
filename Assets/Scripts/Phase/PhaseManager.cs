using UnityEngine;

/// <summary>
/// Phase Data를 관리하며, 각 Data에 따라 적을 스폰하는 역할을 하는 클래스
/// </summary>
public class PhaseManager : Singleton<PhaseManager>
{
    private int currentPhaseIndex = 0;

    [SerializeField]
    private PhaseData[] phaseDatas;

    private void Awake()
    {
        InitSingleton();
    }
}
