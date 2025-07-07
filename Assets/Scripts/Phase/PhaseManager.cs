using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase Data를 관리하며, 각 Data에 따라 적을 스폰하는 역할을 하는 클래스
/// </summary>
public class PhaseManager : Singleton<PhaseManager>
{
    private const int NumSpawnPoints = 8;

    private int phaseIndex = 0;
    private int spawnTimeIndex = 0;

    private float currentPhaseStartTime;
    private float nextPhaseStartTime;

    private int[] currentPhaseSpawnTimes;

    public Transform[] enemySpawnPoints; // 총 8방향

    private PhaseData currentPhaseData;
    [SerializeField]
    private PhaseData[] phaseDatas;

    private void Awake()
    {
        InitSingleton();
    }

    private void Start()
    {
        InitPhaseManager();
    }

    private void InitPhaseManager()
    {
        // Phase Data 배열 유효성 검사
        if (phaseDatas == null || phaseDatas.Length == 0)
        {
            Debug.LogError("Phase Data is not set or empty!");
            return;
        }

        // 필드 초기화
        phaseIndex = 0;
        spawnTimeIndex = 0;
        nextPhaseStartTime = phaseDatas[phaseIndex + 1].phaseStartTime;

        // enemy를 스폰할 Transform 배열 유효성 검사
        if (enemySpawnPoints == null || enemySpawnPoints.Length < NumSpawnPoints)
        {
            Debug.LogError("Spawn points are not set or insufficient!");
            return;
        }
    }

    public void UpdatePhase(float gameTime)
    {
        if (phaseIndex >= phaseDatas.Length) return;

        if (gameTime >= nextPhaseStartTime)
        {
            SetPhase(++phaseIndex);
        }

        if (gameTime >= currentPhaseStartTime + currentPhaseSpawnTimes[spawnTimeIndex])
        {
            
            spawnTimeIndex++;
        }

        
    }

    private void SpawnEnemy(Enemy enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab == null || spawnPoint == null) return;
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void SpawnEnemies()
    {}

    private void SetPhase(int phaseIndex)
    {
        if (phaseIndex >= phaseDatas.Length)
        {
            Debug.Log("All Phases Completed");
            return;
        }
        else
        {
            currentPhaseData = phaseDatas[phaseIndex];
            if (currentPhaseData == null || currentPhaseData.enemies == null || currentPhaseData.spawnTime == null)
            {
                Debug.LogWarning("유효하지 않은 Phase Data 입니다.");
                return;
            }

            currentPhaseStartTime = currentPhaseData.phaseStartTime;
            nextPhaseStartTime = phaseDatas[phaseIndex + 1].phaseStartTime;
            currentPhaseSpawnTimes = currentPhaseData.spawnTime;
            spawnTimeIndex = 0;

            return;
        }
    }
}
