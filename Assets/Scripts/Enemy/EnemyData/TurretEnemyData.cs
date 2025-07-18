using UnityEngine;

// Turret 타입 적 데이터 (고정 위치에서 투사체 발사)
[CreateAssetMenu(fileName = "TurretEnemyData", menuName = "My Scriptable Objects/Enemy Data/Turret Data")]
[System.Serializable]
public class TurretEnemyData : EnemyData
{
    [Header("Projectile Settings")]
    public GameObject enemyBulletPrefab;
    
    private void OnEnable()
    {
        enemyType = EnemyType.Turret;
    }
}
