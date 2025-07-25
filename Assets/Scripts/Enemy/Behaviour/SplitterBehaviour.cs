using UnityEngine;

public class SplitterBehaviour : IEnemyBehavior
{
    public bool hasSplit = false; // 부모 Splitter가 분열했는지 구분하는 플래그
    public bool isSplitChild = false; // 분열체인지 구분하는 플래그
    private float initialVelocityTime = 0f;
    private bool hasStoppedInitialVelocity = false; // 분열 속도 정지 여부 플래그
    private const float INITIAL_VELOCITY_DURATION = 0.3f; // 분열 속도 지속 시간

    private const float splitVelocity = 5f;
    public Vector3 splitDirection;

    Rigidbody _rigidbody;

    public SplitterBehaviour()
    {
    }

    public SplitterBehaviour(bool isSplitChild)
    {
        this.isSplitChild = isSplitChild;
        if (isSplitChild)
        {
            hasSplit = true; // 분열체는 더 이상 분열할 수 없음
            initialVelocityTime = 0f; // 분열 속도 시간 리셋
            hasStoppedInitialVelocity = false; // 초기 속도 정지 플래그 리셋
        }
    }

    public void Execute(Enemy enemy, Vector3 directionToPlayer)
    {
        _rigidbody = enemy._Rigidbody;

        // 게임이 일시정지되어 속도가 0인 경우
        if (isSplitChild && _rigidbody.linearVelocity == Vector3.zero && !hasStoppedInitialVelocity)
        {
            // 분열 속도 시간이 시작되지 않았다면 분열 속도 시간 시작
            if (isSplitChild && initialVelocityTime == 0f)
            {
                _rigidbody.linearVelocity = splitDirection * splitVelocity; // 분열 속도 설정
                Debug.Log($"Splitter: 초기 속도 시간 시작 ({_rigidbody.linearVelocity})");
            }
        }

        // 분열체의 분열 속도 시간이 지나지 않았다면 분열 속도만 유지
        if (isSplitChild && initialVelocityTime < INITIAL_VELOCITY_DURATION)
        {
            initialVelocityTime += Time.deltaTime;
            // 분열 속도가 끝나갈 때쯤 속도를 서서히 줄임
            if (initialVelocityTime >= INITIAL_VELOCITY_DURATION * 0.8f)
            {
                float fadeRatio = 1f - ((initialVelocityTime - INITIAL_VELOCITY_DURATION * 0.8f) / (INITIAL_VELOCITY_DURATION * 0.2f));
                _rigidbody.linearVelocity = _rigidbody.linearVelocity * fadeRatio;
            }
            return; // 분열 속도가 적용된 상태에서는 추가 이동 명령을 주지 않음
        }

        // 분열체의 분열 속도를 한 번만 제거 (플레이어 추적 시작 전)
        if (isSplitChild && !hasStoppedInitialVelocity && initialVelocityTime >= INITIAL_VELOCITY_DURATION)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            hasStoppedInitialVelocity = true;
        }

        // 분열 속도 시간이 지났거나 원본 적이라면 정상적으로 플레이어 추적
        _rigidbody.MovePosition(_rigidbody.position + directionToPlayer * enemy._EnemyData.moveSpeed * Time.deltaTime);
    }

    public void HandleSplitting(Enemy enemy)
    {
        // 이미 분열했거나 분열체라면 분열하지 않음
        if (hasSplit || isSplitChild) return;

        Split(enemy);
        hasSplit = true;
    }

    // 분열체로 설정하는 메서드
    public void SetAsSplitChild()
    {
        isSplitChild = true;
        hasSplit = true; // 분열체는 더 이상 분열할 수 없음
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
            // 부모 Enemy 오브젝트를 복사하여 분열된 자식 Enemy 생성
            GameObject childSplitter = Object.Instantiate(enemy.gameObject, enemy.transform.position, enemy.transform.rotation);

            childSplitter.transform.localScale = enemy.transform.localScale * smallerScale;

            Enemy childEnemyComponent = childSplitter.GetComponent<Enemy>();
            if (childEnemyComponent == null)
            {
                Debug.LogWarning($"SplitterBehaviour: '{enemy.name}'의 자식 오브젝트에 Enemy 컴포넌트가 없습니다.");
                continue;
            }
            
            // 1. 새로운 EnemyData 인스턴스 생성하여 능력치 하향 조정 (원본 수정 방지)
            childEnemyComponent._EnemyData = Object.Instantiate(enemy._EnemyData);
            childEnemyComponent._EnemyData.maxHealth *= splitterData.splitHealthMultiplier;
            childEnemyComponent._EnemyData.attackDamage *= splitterData.splitDamageMultiplier;
            childEnemyComponent._EnemyData.expReward *= splitterData.splitExpMultiplier;
            childEnemyComponent.CurrentHealth = childEnemyComponent._EnemyData.maxHealth;

            // 2. 분열된 적이 다시 분열하지 않도록 새로운 Behaviour 생성 (재귀 방지)
            SplitterBehaviour splitBehaviour = new SplitterBehaviour(true); // 분열체로 생성
            childEnemyComponent.Behavior = splitBehaviour;

            // 3. 분열된 적들이 서로 다른 방향으로 이동하도록 초기 속도 부여
            Rigidbody splitRb = childSplitter.GetComponent<Rigidbody>();
            if (splitRb != null)
            {
                float angle = i * (360f / splitCount);
                splitBehaviour.splitDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                splitRb.linearVelocity = splitBehaviour.splitDirection * splitVelocity; // 초기 속도
            }

            // 분열된 자식 Enemy를 등록
            if (PhaseManager.Instance != null)
            {
                PhaseManager.Instance.RegisterEnemy(childEnemyComponent);
            }
        }
    }
}
