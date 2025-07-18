using UnityEngine;

// Splitter 타입 적 데이터 (죽을 때 분열)
[CreateAssetMenu(fileName = "SplitterEnemyData", menuName = "My Scriptable Objects/Enemy Data/Splitter Data")]
[System.Serializable]
public class SplitterEnemyData : EnemyData
{
    [Header("Split Settings")]
    [Range(2, 4)] public int splitCount = 2;
    [Range(0.3f, 0.8f)] public float splitSizeMultiplier = 0.7f;
    [Range(0.3f, 0.8f)] public float splitHealthMultiplier = 0.5f;
    [Range(0.3f, 0.8f)] public float splitDamageMultiplier = 0.7f;
    [Range(0.3f, 0.8f)] public float splitExpMultiplier = 0.5f;
    
    private void OnEnable()
    {
        enemyType = EnemyType.Splitter;
    }
}
