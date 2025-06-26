using UnityEngine;

// <summary>
// 게임 내 적의 기본적인 속성과 행동을 정의하는 모든 적의 부모 클래스
public class Enemy : Entity
{
    private float _maxHealth;
    private float _moveSpeed;
    private float _attackDamage;
    private float _attackSpeed;
    private float _attackRange;
    private float _detectionRange;
    private float _currentHealth;
    private float _lastAttackTime;

    private static Vector3 _targetPosition;

    private IEnemyBehavior behavior;

    private Rigidbody _rigidbody;

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

    [SerializeField] public EnemyData _EnemyData { get; private set; }

    private void Awake()
    {
        InitEnemy();
    }

    private void Start()
    {
        behavior = _EnemyData.enemyType switch
        {
            EnemyType.Turret => new TurretBehaviour(),
            EnemyType.Charger => new ChargerBehaviour(),
            EnemyType.Shooter => new ShooterBehaviour(),
            _ => new ChargerBehaviour()
        };
    }

    private void Update()
    {
        if (!IsAlive) return;

        Vector3 directionToPlayer = (_Player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, _Player.transform.position);

        if (distanceToPlayer <= _detectionRange)
        {
            behavior?.Execute(this, _Player.transform);
        }
    }

    public void InitEnemy()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        _maxHealth = _EnemyData.maxHealth;
        _moveSpeed = _EnemyData.moveSpeed;
        _attackDamage = _EnemyData.attackDamage;
        _attackSpeed = _EnemyData.attackSpeed;
        _attackRange = _EnemyData.attackRange;
        _detectionRange = _EnemyData.detectionRange;
        _currentHealth = _maxHealth;
    }

    public void Move(Vector2 direction)
    {
        if (_targetPosition == null || _rigidbody == null || _EnemyData.enemyType == EnemyType.Turret)
            return;

        _rigidbody.MovePosition(_rigidbody.position + (Vector3)direction * _EnemyData.moveSpeed * Time.deltaTime);
        // Implement movement logic here
    }

    public override void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0 && IsAlive)
        {
            Die();
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
        gameObject.SetActive(false);
        // 추가적인 사망 처리 (파티클 효과, 사운드 등)
    }
}
