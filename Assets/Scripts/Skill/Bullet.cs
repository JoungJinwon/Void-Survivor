using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const int DefaultBulletSpeed = 40;
    private const float MaxBulletSpeed = 100f; // 최대 속도 제한
    private Vector3 moveDirection;
    private float moveSpeed;
    private int damage;
    private Vector3 previousPosition;
    private float lifeTime = 5f;
    private float currentLifeTime = 0f;

    public void Init(Vector3 direction, float speed, int damage)
    {
        moveDirection = direction.normalized;
        // 속도 제한 적용
        float calculatedSpeed = speed * DefaultBulletSpeed;
        moveSpeed = Mathf.Min(calculatedSpeed, MaxBulletSpeed);
        this.damage = damage;
        
        // 이전 위치 초기화
        previousPosition = transform.position;
    }

    void Update()
    {
        // 생명 시간 체크
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // 이동 전 위치 저장
        previousPosition = transform.position;
        
        // 총알 이동
        Vector3 moveDistance = moveDirection * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + moveDistance;
        
        // Raycast로 연속 충돌 감지
        if (CheckCollisionWithRaycast(moveDistance.magnitude))
        {
            return; // 충돌 시 총알이 이미 파괴되었으므로 리턴
        }
        
        // 충돌하지 않았다면 위치 업데이트
        transform.position = newPosition;
    }

    /// <summary>
    /// Raycast를 이용한 연속 충돌 감지
    /// </summary>
    /// <param name="moveDistance">이동 거리</param>
    /// <returns>충돌 여부</returns>
    private bool CheckCollisionWithRaycast(float moveDistance)
    {
        // 현재 위치에서 이동 방향으로 레이캐스트
        RaycastHit[] hits = Physics.RaycastAll(previousPosition, moveDirection, moveDistance);
        
        // 가장 가까운 충돌 지점 찾기
        RaycastHit closestHit = default;
        float closestDistance = float.MaxValue;
        bool hasValidHit = false;
        
        foreach (var hit in hits)
        {
            // 자기 자신과의 충돌은 무시
            if (hit.collider.gameObject == gameObject) continue;
            
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestHit = hit;
                hasValidHit = true;
            }
        }
        
        // 충돌이 있다면 처리
        if (hasValidHit)
        {
            // 충돌 지점으로 총알 위치 이동
            transform.position = closestHit.point;
            
            // 충돌 처리
            HandleCollision(closestHit.collider);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 충돌 처리 메서드
    /// </summary>
    /// <param name="collider">충돌한 콜라이더</param>
    private void HandleCollision(Collider collider)
    {
        // Enemy와 충돌 시 데미지 적용
        if (collider.TryGetComponent(out Enemy enemy))
        {
            if (enemy.IsAlive)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // 벽 등 다른 오브젝트와 충돌 시에도 파괴 (Trigger가 아닌 경우)
        else if (!collider.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 백업용 충돌 처리 (혹시 Raycast가 놓친 경우를 대비)
        HandleCollision(other);
    }
}
