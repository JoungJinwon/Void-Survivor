using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/AccelerationZoneSkill")]
public class AccelerationZoneSkill : Skill
{
    private const int randomPosOffset = 30;

    private float timer;

    public float attackSpeedBoost;
    public float moveSpeedBoost;

    public float zoneRadius;
    public float zoneDuration;

    public GameObject accelerationZonePrefab;

    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialAttackSpeedBoost;
    [HideInInspector] public float initialMoveSpeedBoost;
    [HideInInspector] public float initialZoneRadius;
    [HideInInspector] public float initialZoneDuration;

    public override void Activate()
    {
        base.Activate();

        cooldownTime = 10f; // 활성화 시 쿨다운 설정
        timer = 0f;
        
        SkillManager.Instance.RegisterUpdate(UpdateSkill);

        InstantiateAccelerationZone();
    }

    private void UpdateSkill()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTime)
        {
            timer = 0f;
            InstantiateAccelerationZone();
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();

        attackSpeedBoost += 0.05f;
        moveSpeedBoost += 0.05f;
        zoneRadius += 2f;
        zoneDuration += 1f;
        cooldownTime -= 0.5f;
    }

    private void InstantiateAccelerationZone()
    {
        Vector3 randomPos = GameManager.Instance._Player.transform.position + (Vector3)(Random.insideUnitCircle * randomPosOffset);
        randomPos.y = 5f; // y축은 5 고정
        GameObject zone = Instantiate(accelerationZonePrefab, randomPos, Quaternion.identity);
        zone.GetComponent<AccelerationZone>().Init(zoneRadius, zoneDuration, attackSpeedBoost, moveSpeedBoost);
    }
    
        
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;
        
        base.StoreInitialValues();

        initialAttackSpeedBoost = attackSpeedBoost;
        initialMoveSpeedBoost = moveSpeedBoost;
        initialZoneRadius = zoneRadius;
        initialZoneDuration = zoneDuration;
    }
    
    public override void ResetToInitialValues()
    {
        if (!hasStoredInitialValues) return;

        base.ResetToInitialValues();

        attackSpeedBoost = initialAttackSpeedBoost;
        moveSpeedBoost = initialMoveSpeedBoost;
        zoneRadius = initialZoneRadius;
        zoneDuration = initialZoneDuration;
        timer = 0f;
    }
}
