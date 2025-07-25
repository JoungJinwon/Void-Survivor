using UnityEngine;

// Bomber 타입 적 데이터 (폭발 공격)
[CreateAssetMenu(fileName = "BomberEnemyData", menuName = "My Scriptable Objects/Enemy Data/Bomber Data")]
[System.Serializable]
public class BomberEnemyData : EnemyData
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 20f;

    public AudioClip explosionSound;
    public GameObject explosionEffectPrefab;
    
    private void OnEnable()
    {
        enemyType = EnemyType.Bomber;
    }
}
