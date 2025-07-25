using UnityEngine;

// <summary>
// 여러 타입의 적 각각에 해당하는 정보를 저장하기 위한 기본 Scriptable Object Class
[System.Serializable]
public abstract class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public float attackDamage;
    public float attackCoolTime;
    public float detectionRange;
    public float expReward;
    public EnemyType enemyType;
}
