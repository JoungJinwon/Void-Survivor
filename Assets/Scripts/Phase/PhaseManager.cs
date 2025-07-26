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
    private int spawnInfoIndex = 0;

    // Phase가 시작된 시점의 Game Time
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
    private List<PhaseData.SpawnInfo> currentPhaseSpawnInfos;

    private void Awake()
    {
        InitSingleton();
    }

    private void Start()
    {
        InitPhaseManager();
        ResetPhaseManager();
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

    /// <summary>
    /// Phase Manager를 초기 상태로 재설정합니다. 씬 재시작 시 사용됩니다.
    /// </summary>
    public void ResetPhaseManager()
    {
        IsPhaseActive = false;
        phaseIndex = 0;
        spawnInfoIndex = 0;
        currentPhaseStartTime = 0f;
        enemyCount = 0;
        
        // 기존 적 리스트 초기화
        remainingEnemies.Clear();
        
        currentPhaseData = null;
        currentPhaseSpawnInfos = null;
        
        Debug.Log("Phase Manager Reset Successfully");
    }

    public void UpdatePhase(float gameTime)
    {
        if (!IsPhaseActive || currentPhaseData == null || GameManager.Instance.IsGamePaused) return;

        if (spawnInfoIndex < currentPhaseSpawnInfos.Count)
        {
            var spawnInfo = currentPhaseSpawnInfos[spawnInfoIndex];

            // 스폰 시간이 되었는지 확인
            if (gameTime >= currentPhaseStartTime + spawnInfo.spawnTime)
            {
                SpawnEnemies(spawnInfo.enemies);
                spawnInfoIndex++;
            }
            else if (enemyCount == 0)
            {
                float remainingTime = (currentPhaseStartTime + spawnInfo.spawnTime) - gameTime;
                currentPhaseStartTime += remainingTime;

                SpawnEnemies(spawnInfo.enemies);
                spawnInfoIndex++;
            }
        }
        else if (enemyCount == 0)
        {
            SetPhase(phaseIndex + 1);
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab, Transform spawnPoint)
    {
        float spawnOffset = Random.Range(-15f, 15f);
        Vector3 newSpawnPoint = spawnPoint.position + new Vector3(spawnOffset, 0, spawnOffset);

        Enemy newEnemy = Instantiate(enemyPrefab, newSpawnPoint, spawnPoint.rotation);
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

    // 외부에서 enemy를 등록하는 함수
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null || remainingEnemies.Contains(enemy)) return;

        remainingEnemies.Add(enemy);
        enemyCount++;
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
        currentPhaseSpawnInfos = currentPhaseData.spawnInfos;
        spawnInfoIndex = 0;
        IsPhaseActive = true;

        UiManager.Instance.UpdatePhaseText(currentPhaseData.phaseName);
    }

    public string GetCurrentPhaseName()
    {
        return currentPhaseData != null ? currentPhaseData.phaseName : "No Phase Active";
    }

    public void DecreaseEnemyCount()
    {
        if (enemyCount > 0)
            enemyCount--;
    }

    // 죽은 적을 리스트에서 제거하는 메서드 추가
    public void RemoveEnemy(Enemy enemy)
    {
        if (enemy != null && remainingEnemies.Contains(enemy))
        {
            remainingEnemies.Remove(enemy);
            DecreaseEnemyCount();
        }
    }
}