using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase Data를 관리하며, 각 Data에 따라 적을 스폰하는 역할을 하는 클래스
/// </summary>
public class PhaseManager : Singleton<PhaseManager>
{
    private const int NumSpawnPoints = 8;

    private bool IsPhaseActive = false;

    private int phaseIndex = 0;
    private int spawnTimeIndex = 0;

    private float currentPhaseStartTime;

    // 총 8방향의 Enemy Spawners
    public Transform[] enemySpawnPoints;

    private PhaseData currentPhaseData;
    [SerializeField]
    private PhaseData[] phaseDatas;

    // 현재 Phase의 남아있는 적 수
    private int enemyCount = 0;
    private List<Enemy> remainingEnemies = new List<Enemy>();

    // 현재 Phase의 스폰 정보
    private List<PhaseData.SpawnInfo> currentSpawnInfos;

    private void Awake()
    {
        InitSingleton();
    }

    private void Start()
    {
        InitPhaseManager();
        SetPhase(0);
    }

    private void InitPhaseManager()
    {
        // Phase Data 배열 유효성 검사
        if (phaseDatas == null || phaseDatas.Length == 0)
        {
            Debug.LogError("Phase Data is not set or empty!");
            return;
        }

        // enemy를 스폰할 Transform 배열 유효성 검사
        if (enemySpawnPoints == null || enemySpawnPoints.Length < NumSpawnPoints)
        {
            Debug.LogError("Spawn points are not set or insufficient!");
            return;
        }
    }

    public void UpdatePhase(float gameTime)
    {
        if (!IsPhaseActive || currentPhaseData == null || GameManager.Instance.IsGamePaused) return;

        // 모든 스폰 타임을 처리했는지 확인
        if (spawnTimeIndex >= currentSpawnInfos.Count)
        {
            // 남아있는 적이 없으면 다음 Phase로
            if (enemyCount == 0)
            {
                SetPhase(phaseIndex + 1);
            }

            return;
        }

        var spawnInfo = currentSpawnInfos[spawnTimeIndex];
        if (gameTime >= currentPhaseStartTime + spawnInfo.spawnTime)
        {
            SpawnEnemies(spawnInfo.enemies);
            spawnTimeIndex++;
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab == null || spawnPoint == null) return;
        Enemy newEnemy =Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        remainingEnemies.Add(newEnemy);
        enemyCount++;
    }

    private void SpawnEnemies(List<PhaseData.EnemySpawn> enemiesToSpawn)
    {
        foreach (var enemySpawn in enemiesToSpawn)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                int spawnPointIndex = Random.Range(0, NumSpawnPoints);
                SpawnEnemy(enemySpawn.enemyPrefab, enemySpawnPoints[spawnPointIndex]);
            }
        }
    }

    public List<Enemy> GetRemainingEnemies()
    {
        return remainingEnemies;
    }

    private void SetPhase(int newPhaseIndex)
    {
        if (newPhaseIndex >= phaseDatas.Length)
        {
            Debug.Log("모든 Phase를 클리어했습니다.");
            IsPhaseActive = false;
            return;
        }

        phaseIndex = newPhaseIndex;
        currentPhaseData = phaseDatas[phaseIndex];
        if (currentPhaseData == null || currentPhaseData.spawnInfos == null)
        {
            Debug.LogWarning("유효하지 않은 Phase Data 입니다.");
            IsPhaseActive = false;
            return;
        }

        currentPhaseStartTime = GameManager.Instance.GameTime;
        currentSpawnInfos = currentPhaseData.spawnInfos;
        spawnTimeIndex = 0;
        IsPhaseActive = true;
    }

    public void DecreaseEnemyCount()
    {
        if (enemyCount > 0)
            enemyCount--;
    }
}