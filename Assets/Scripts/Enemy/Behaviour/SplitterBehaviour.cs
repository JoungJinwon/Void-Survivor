using UnityEngine;

public class SplitterBehaviour : IEnemyBehavior
{
    private bool hasSplit = false;
    
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        if (GameManager.Instance.IsGamePaused)
            return;
        
        // 기본적으로는 charger처럼 이동
        Rigidbody rb = enemy._Rigidbody;
        rb.MovePosition(rb.position + directionToPlayer * enemy._EnemyData.moveSpeed * Time.deltaTime);
    }
    
    public void HandleSplitting(Enemy enemy)
    {
        // 이미 분열했다면 분열하지 않음
        if (hasSplit) return;
        
        Split(enemy);
        hasSplit = true;
    }
    
    private void Split(Enemy enemy)
    {
        // SplitterEnemyData에서 분열 설정값 가져오기
        if (!(enemy._EnemyData is SplitterEnemyData splitterData))
        {
            Debug.LogWarning($"SplitterBehaviour: Enemy '{enemy.name}'의 EnemyData가 SplitterEnemyData가 아닙니다.");
            return;
        }
        
        int splitCount = splitterData.splitCount;
        float smallerScale = splitterData.splitSizeMultiplier;
        
        for (int i = 0; i < splitCount; i++)
        {
            // 분열된 적 생성
            GameObject splitEnemy = Object.Instantiate(enemy.gameObject, enemy.transform.position, enemy.transform.rotation);
            
            // 크기 조정
            splitEnemy.transform.localScale = enemy.transform.localScale * smallerScale;
            
            // 분열된 적의 체력과 공격력 조정
            Enemy splitEnemyComponent = splitEnemy.GetComponent<Enemy>();
            if (splitEnemyComponent != null)
            {
                // 새로운 EnemyData 인스턴스 생성 (원본 수정 방지)
                splitEnemyComponent._EnemyData = Object.Instantiate(enemy._EnemyData);
                splitEnemyComponent._EnemyData.maxHealth *= splitterData.splitHealthMultiplier;
                splitEnemyComponent._EnemyData.attackDamage *= splitterData.splitDamageMultiplier;
                splitEnemyComponent._EnemyData.expReward *= splitterData.splitExpMultiplier;
                splitEnemyComponent.CurrentHealth = splitEnemyComponent._EnemyData.maxHealth;
                
                // 분열된 적이 다시 분열하지 않도록 처리 (재귀 방지)
                SplitterBehaviour splitBehaviour = splitEnemyComponent.Behavior as SplitterBehaviour;
                if (splitBehaviour != null)
                {
                    splitBehaviour.hasSplit = true; // 이미 분열했다고 마킹
                }
            }
            
            // 분열된 적들이 서로 다른 방향으로 이동하도록 초기 속도 부여
            Rigidbody splitRb = splitEnemy.GetComponent<Rigidbody>();
            if (splitRb != null)
            {
                float angle = i * (360f / splitCount);
                Vector3 splitDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                splitRb.linearVelocity = splitDirection * 5f; // 초기 속도
            }
            
            // PhaseManager에 새로운 적 추가
            if (PhaseManager.Instance != null)
            {
                PhaseManager.Instance.RegisterEnemy(splitEnemyComponent);
            }
        }
    }
}
