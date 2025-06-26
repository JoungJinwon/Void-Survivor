using UnityEngine;

public class SmallCharger : Enemy
{
    private float _chargeDelay = 2f;    // 돌진 전 대기 시간
    private float _chargeSpeed = 10f;    // 돌진 속도
    private float _nextChargeTime;       // 다음 돌진 가능 시간
    private bool _isCharging;            // 현재 돌진 중인지 여부
    private Vector3 _chargeDirection;    // 돌진 방향

    private void Start()
    {
        _nextChargeTime = Time.time + _chargeDelay;
    }

    private void Update()
    {
        if (!IsAlive) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, _Player.transform.position);

        if (!_isCharging)
        {
            // 돌진 준비 상태
            if (Time.time >= _nextChargeTime)
            {
                StartCharge();
            }
            else
            {
                // 돌진 대기 중에는 플레이어 방향으로 천천히 이동
                Vector3 directionToPlayer = (_Player.transform.position - transform.position).normalized;
                Move(directionToPlayer);
            }
        }
        else
        {
            // 돌진 실행
            ExecuteCharge();
        }
    }

    private void StartCharge()
    {
        _isCharging = true;
        // 현재 플레이어 위치를 향해 돌진 방향 설정
        _chargeDirection = (_Player.transform.position - transform.position).normalized;
    }

    private void ExecuteCharge()
    {
        // 돌진 이동 구현
        transform.position += _chargeDirection * _chargeSpeed * Time.deltaTime;

        // 일정 시간 후 돌진 종료
        if (Time.time >= _nextChargeTime + 1f)
        {
            _isCharging = false;
            _nextChargeTime = Time.time + _chargeDelay;
        }
    }
}