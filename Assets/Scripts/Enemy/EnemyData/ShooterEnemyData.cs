using UnityEngine;

// Shooter 타입 적 데이터 (투사체 발사 가능)
[CreateAssetMenu(fileName = "ShooterEnemyData", menuName = "My Scriptable Objects/Enemy Data/Shooter Data")]
[System.Serializable]
public class ShooterEnemyData : EnemyData
{
    [Header("Projectile Settings")]
    public GameObject enemyBulletPrefab;
    
    private void OnEnable()
    {
        enemyType = EnemyType.Shooter;
    }
}
