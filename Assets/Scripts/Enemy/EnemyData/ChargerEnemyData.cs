using UnityEngine;

// Charger 타입 적 데이터
[CreateAssetMenu(fileName = "ChargerEnemyData", menuName = "My Scriptable Objects/Enemy Data/Charger Data")]
[System.Serializable]
public class ChargerEnemyData : EnemyData
{
    private void OnEnable()
    {
        enemyType = EnemyType.Charger;
    }
}
