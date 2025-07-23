using UnityEngine;

public class BomberBehaviour : IEnemyBehavior
{
    private bool hasExploded = false;
    private float explosionRange = 3f;
    private float explosionDamage = 50f;
    private float fuseTime = 1f; // 폭발까지의 시간
    private float currentFuseTime = 0f;
    private bool fuseActivated = false;
    
    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        if (GameManager.Instance.IsGamePaused || hasExploded)
            return;
        
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.transform.position + directionToPlayer * 100f);
        
        // 플레이어와의 실제 거리 계산
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            distanceToPlayer = Vector3.Distance(enemy.transform.position, playerPosition);
        }
        
        // 폭발 범위 안에 들어오면 퓨즈 활성화
        if (distanceToPlayer <= explosionRange && !fuseActivated)
        {
            fuseActivated = true;
            // 시각적 효과 (빨간색 깜빡임 등)
            enemy.TriggerSpecialEffect("burn", fuseTime);
        }
        
        // 퓨즈가 활성화되면 카운트다운 시작
        if (fuseActivated)
        {
            currentFuseTime += Time.deltaTime;
            
            if (currentFuseTime >= fuseTime)
            {
                Explode(enemy);
                return;
            }
        }
        
        // 기본적으로는 플레이어를 향해 이동 (더 빠른 속도)
        Rigidbody rb = enemy._Rigidbody;
        float moveSpeed = enemy._EnemyData.moveSpeed * (fuseActivated ? 1.5f : 1f); // 퓨즈 활성화시 더 빠르게
        rb.MovePosition(rb.position + directionToPlayer * moveSpeed * Time.deltaTime);
    }
    
    private void Explode(Enemy enemy)
    {
        if (hasExploded) return;
        hasExploded = true;
        
        // 폭발 범위 내의 모든 콜라이더 찾기
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, explosionRange);
        
        foreach (Collider hitCollider in hitColliders)
        {
            // 플레이어에게 데미지
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                }
            }
            
            // 다른 적들에게도 데미지 (선택사항)
            else if (hitCollider.CompareTag("Enemy"))
            {
                Enemy otherEnemy = hitCollider.GetComponent<Enemy>();
                if (otherEnemy != null && otherEnemy != enemy)
                {
                    otherEnemy.TakeDamage(explosionDamage * 0.5f); // 다른 적에게는 절반 데미지
                }
            }
        }
        
        // 폭발 파티클 효과 (있다면)
        // ParticleSystem explosionEffect = enemy.GetComponent<ParticleSystem>();
        // if (explosionEffect != null)
        // {
        //     explosionEffect.Play();
        // }
        
        // 폭발 시각 효과
        enemy.TriggerSpecialEffect("electric", 0.2f);
        
        // 자폭하므로 적 자신도 죽음
        enemy.CurrentHealth = 0;
        // Enemy.Die()가 호출될 것임
    }
    
    // 디버그용 - 폭발 범위 시각화
    public void OnDrawGizmosSelected(Enemy enemy)
    {
        if (enemy != null)
        {
            Gizmos.color = fuseActivated ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(enemy.transform.position, explosionRange);
        }
    }
}
