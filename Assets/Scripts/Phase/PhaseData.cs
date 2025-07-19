using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase_Data", menuName = "My Scriptable Objects/PhaseData")]
public class PhaseData : ScriptableObject
{
    public string phaseName;

    [Serializable]
    public class SpawnInfo
    {
        public int spawnTime; // 현재 스테이지가 시작된 시점을 기준으로 몇 초 후에 스폰할지?
        public List<EnemySpawn> enemies; // 해당 시간에 스폰할 적 리스트
    }

    /// <summary>
    /// 각 스폰 타임별로 스폰할 적 프리팹과 수를 저장하는 클래스
    /// </summary>
    [Serializable]
    public class EnemySpawn
    {
        public Enemy enemyPrefab;
        public int count;
    }

    // 인스펙터에서 시간별로 적 지정
    public List<SpawnInfo> spawnInfos;
}
