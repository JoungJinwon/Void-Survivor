using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// <summary>
// 게임 내 적의 기본적인 속성과 행동을 정의하는 모든 적의 부모 클래스
public class Enemy : Entity
{
    private float _maxHealth;
    private float _currentHealth;
    private float _attackDamage;
    private float _attackCoolTime;
    private float _lastAttackTime;

    private HealthBar hpBar;

    protected Vector3 directionToPlayer;

    protected IEnemyBehavior behavior;

    public Rigidbody _Rigidbody { get; private set; }

    // 공용 접근자 추가
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public IEnemyBehavior Behavior { get { return behavior; } set { behavior = value; } }

    // 시각적 효과를 위한 변수들
    [Header("Visual Effects")]
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private ParticleSystem deathParticles;
    
    private Renderer enemyRenderer;
    private bool isDying = false;
    private bool isFlashing = false;

    private static Player _player;
    protected static Player _Player
    {
        get
        {
            if (_player == null)
                _player = FindFirstObjectByType<Player>();
            return _player;
        }
    }

    [SerializeField] public EnemyData _EnemyData;

    protected virtual void Awake()
    {
        InitEnemy();
    }

    protected virtual void Start()
    {
        behavior = _EnemyData.enemyType switch
        {
            EnemyType.Charger => new ChargerBehaviour(),
            EnemyType.Shooter => new ShooterBehaviour(),
            EnemyType.Turret => new TurretBehaviour(),
            EnemyType.Splitter => new SplitterBehaviour(),
            EnemyType.Bomber => new ChargerBehaviour(), // Bomber는 일단 Charger처럼 행동
            _ => new ChargerBehaviour()
        };
    }

    protected virtual void Update()
    {
        if (!IsAlive || GameManager.Instance.IsGamePaused) return;

        directionToPlayer = (_Player.transform.position - transform.position).normalized;

        behavior?.Execute(this, directionToPlayer);
        
        // 체력에 따른 시각적 업데이트
        UpdateHealthVisuals();
    }

    public void InitEnemy()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();

        _maxHealth = _EnemyData.maxHealth;
        _attackDamage = _EnemyData.attackDamage;
        _attackCoolTime = _EnemyData.attackCoolTime;
        _currentHealth = _maxHealth;

        // 머티리얼 복사 (인스턴스 생성)
        if (enemyRenderer != null && enemyMaterial != null)
        {
            enemyRenderer.material = new Material(enemyMaterial);
            enemyMaterial = enemyRenderer.material;
        }
    }

    public override void TakeDamage(float amount)
    {
        if (isDying) return;

        _currentHealth -= amount;

        if (_currentHealth <= 0 && IsAlive)
        {
            Die();
        }
        else
        {
            // 데미지 시각적 효과
            StartCoroutine(DamageVisualEffect());
            
            if (hpBar == null || !HealthBarManager.Instance.IsBarActive(hpBar))
            {
                hpBar = HealthBarManager.Instance.RequestBar(transform);
                Debug.Log($"{gameObject.name}의 체력바 초기화");
            }

            hpBar.UpdateHealth(_currentHealth / _maxHealth);
        }
    }

    public void Attack(IEntity target)
    {
        if (Time.time - _lastAttackTime < _attackCoolTime) return;

        if (target is Player player)
        {
            player.TakeDamage(_attackDamage);
            _lastAttackTime = Time.time;
        }
    }

    protected override void Die()
    {
        if (isDying) return;
        
        // Splitter 타입인 경우 분열 처리
        if (_EnemyData.enemyType == EnemyType.Splitter && behavior is SplitterBehaviour splitterBehaviour)
        {
            splitterBehaviour.HandleSplitting(this);
        }
        
        isDying = true;

        IsAlive = false;

        if (HealthBarManager.Instance.IsBarActive(hpBar))
            hpBar.Deactivate();

        _player.GainExp(_EnemyData.expReward);

        _Rigidbody.linearVelocity = Vector3.zero;

        // 사망 시각적 효과 시작
        StartCoroutine(DeathVisualEffect());

        PhaseManager.Instance.DecreaseEnemyCount();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(_player);
        }
    }

    // 시각적 효과 코루틴들
    private IEnumerator DamageVisualEffect()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        // 셰이더 데미지 효과
        if (enemyMaterial != null)
        {
            enemyMaterial.SetFloat("_DamageAmount", 1.0f);
            
            // 펄스 효과 증가
            float originalPulseSpeed = enemyMaterial.GetFloat("_PulseSpeed");
            enemyMaterial.SetFloat("_PulseSpeed", originalPulseSpeed * 2.0f);
            
            // 림 라이팅 강화
            Color originalRimColor = enemyMaterial.GetColor("_RimColor");
            enemyMaterial.SetColor("_RimColor", Color.red);
            
            // 0.1초 후 원래 상태로 복구
            yield return new WaitForSeconds(0.1f);
            
            enemyMaterial.SetFloat("_DamageAmount", 0.0f);
            enemyMaterial.SetFloat("_PulseSpeed", originalPulseSpeed);
            enemyMaterial.SetColor("_RimColor", originalRimColor);
        }

        isFlashing = false;
    }

    private IEnumerator DeathVisualEffect()
    {
        // 디졸브 효과로 사망 연출
        if (enemyMaterial != null)
        {
            float dissolveTime = 0.5f;
            float elapsedTime = 0f;
            
            // 발광 효과 증가
            Color originalEmission = enemyMaterial.GetColor("_EmissionColor");
            enemyMaterial.SetColor("_EmissionColor", Color.white);
            enemyMaterial.SetFloat("_EmissionIntensity", 3.0f);
            
            while (elapsedTime < dissolveTime)
            {
                float dissolveAmount = elapsedTime / dissolveTime;
                enemyMaterial.SetFloat("_DissolveAmount", dissolveAmount);
                
                // 디졸브 진행에 따라 사망 파티클 생성
                if (dissolveAmount > 0.5f && deathParticles != null && !deathParticles.isPlaying)
                {
                    deathParticles.Play();
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // 머티리얼이 없는 경우 간단한 페이드 아웃
            yield return new WaitForSeconds(1.0f);
        }

        // 오브젝트 파괴
        Destroy(gameObject);
    }

    // 체력에 따른 시각적 상태 업데이트
    private void UpdateHealthVisuals()
    {
        if (enemyMaterial == null) return;

        float healthPercent = _currentHealth / _maxHealth;
        
        // 체력이 낮을수록 더 강한 발광 효과
        float emissionIntensity = Mathf.Lerp(1.0f, 3.0f, 1.0f - healthPercent);
        enemyMaterial.SetFloat("_EmissionIntensity", emissionIntensity);
        
        // 체력이 낮을수록 더 빠른 펄스
        float pulseSpeed = Mathf.Lerp(1.0f, 3.0f, 1.0f - healthPercent);
        enemyMaterial.SetFloat("_PulseSpeed", pulseSpeed);
        
        // 체력이 30% 이하일 때 경고 색상
        if (healthPercent <= 0.3f)
        {
            Color warningColor = Color.Lerp(Color.yellow, Color.red, (0.3f - healthPercent) / 0.3f);
            enemyMaterial.SetColor("_RimColor", warningColor);
        }
    }

    private void OnDestroy()
    {
        // 머티리얼 인스턴스 정리
        if (enemyMaterial != null)
        {
            DestroyImmediate(enemyMaterial);
        }
    }

    // 외부에서 호출 가능한 특수 효과 메서드들
    public void TriggerSpecialEffect(string effectName, float duration = 1.0f)
    {
        StartCoroutine(SpecialEffectCoroutine(effectName, duration));
    }

    private IEnumerator SpecialEffectCoroutine(string effectName, float duration)
    {
        if (enemyMaterial == null) yield break;

        switch (effectName.ToLower())
        {
            case "freeze":
                // 얼음 효과
                enemyMaterial.SetColor("_RimColor", Color.cyan);
                enemyMaterial.SetFloat("_RimPower", 1.0f);
                enemyMaterial.SetFloat("_PulseSpeed", 0.2f);
                break;
                
            case "burn":
                // 화염 효과
                enemyMaterial.SetColor("_EmissionColor", Color.red);
                enemyMaterial.SetFloat("_EmissionIntensity", 5.0f);
                enemyMaterial.SetFloat("_PulseSpeed", 5.0f);
                break;
                
            case "electric":
                // 전기 효과
                enemyMaterial.SetColor("_RimColor", Color.yellow);
                enemyMaterial.SetFloat("_RimPower", 0.5f);
                enemyMaterial.SetFloat("_PulseSpeed", 10.0f);
                break;
                
            case "poison":
                // 독 효과
                enemyMaterial.SetColor("_Color", Color.green);
                enemyMaterial.SetFloat("_PulseSpeed", 2.0f);
                break;
        }

        yield return new WaitForSeconds(duration);

        // 원래 상태로 복구 (기본값으로 리셋)
        ResetVisualEffects();
    }

    private void ResetVisualEffects()
    {
        if (enemyMaterial == null) return;

        // 기본값으로 리셋
        enemyMaterial.SetColor("_Color", Color.white);
        enemyMaterial.SetColor("_RimColor", new Color(0, 1, 1, 1));
        enemyMaterial.SetFloat("_RimPower", 2.0f);
        enemyMaterial.SetFloat("_PulseSpeed", 1.0f);
        enemyMaterial.SetColor("_EmissionColor", Color.black);
        enemyMaterial.SetFloat("_EmissionIntensity", 1.0f);
        enemyMaterial.SetFloat("_DamageAmount", 0.0f);
    }
}