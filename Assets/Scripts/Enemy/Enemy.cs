using UnityEngine;
using UnityEngine.UI;

// <summary>
// 게임 내 적의 기본적인 속성과 행동을 정의하는 모든 적의 부모 클래스
public class Enemy : Entity
{
    private float _maxHealth;
    private float _currentHealth;
    private float _moveSpeed;
    private float _attackDamage;
    private float _attackSpeed;
    private float _attackRange;
    private float _detectionRange;
    private float _lastAttackTime;

    private HealthBar hpBar;

    protected Vector3 directionToPlayer;

    protected IEnemyBehavior behavior;

    public Rigidbody _Rigidbody { get; private set; }

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
            _ => new ChargerBehaviour()
        };
    }

    protected virtual void Update()
    {
        if (!IsAlive) return;

        directionToPlayer = (_Player.transform.position - transform.position).normalized;

        
    }

    public void InitEnemy()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        
        _maxHealth = _EnemyData.maxHealth;
        _moveSpeed = _EnemyData.moveSpeed;
        _attackDamage = _EnemyData.attackDamage;
        _attackSpeed = _EnemyData.attackSpeed;
        _attackRange = _EnemyData.attackRange;
        _detectionRange = _EnemyData.detectionRange;
        _currentHealth = _maxHealth;
    }

    public override void TakeDamage(float amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0 && IsAlive)
        {
            Die();
        }
        else
        {
            if (hpBar == null || !HealthBarManager.Instance.IsBarActive(hpBar))
            {
                hpBar = HealthBarManager.Instance.RequestBar(transform);
                Debug.Log($"{gameObject.name}의 체력바 초기화");
            }

            hpBar.UpdateHealth(_currentHealth / _maxHealth);
        }
    }

    public override void Attack(IEntity target)
    {
        if (Time.time - _lastAttackTime < _attackSpeed) return;

        if (target is Player player)
        {
            player.TakeDamage(_attackDamage);
            _lastAttackTime = Time.time;
        }
    }

    protected override void Die()
    {
        IsAlive = false;
        if (HealthBarManager.Instance.IsBarActive(hpBar))
            hpBar.Deactivate();
        Destroy(gameObject);
        // gameObject.SetActive(false);
        // 추가적인 사망 처리 (파티클 효과, 사운드 등)
    }
}