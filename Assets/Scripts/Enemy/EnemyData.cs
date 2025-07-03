using UnityEngine;

// <summary>
// 여러 타입의 적 각각에 해당하는 정보를 저장하기 위한 Scriptable Object Class
[CreateAssetMenu(fileName = "EnemyData", menuName = "My Scriptable Objects/Enemy Data")]
[System.Serializable]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float detectionRange;
    public float expReward;
    public EnemyType enemyType;
}
