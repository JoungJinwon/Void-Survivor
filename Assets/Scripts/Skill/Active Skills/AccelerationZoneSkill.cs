using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/AccelerationZoneSkill")]
public class AccelerationZoneSkill : Skill
{
    private const int randomPosOffset = 15;
    public int attackBoost = 10;
    public float attackSpeedBoost = 1.3f;
    public float moveSpeedBoost = 1.3f;

    public float zoneRadius = 20f;
    public float zoneDuration = 6f;

    public GameObject accelerationZonePrefab;

    private float timer;

    public override void Activate()
    {
        base.Activate();

        cooldownTime = 7f;
        timer = 0f;
        SkillManager.Instance.RegisterUpdate(UpdateSkill);

        InstantiateAccelerationZone();
    }

    void UpdateSkill()
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
        if (skillLevel >= maxLevel) return;

        skillLevel++;

        attackBoost += 5;
        attackSpeedBoost += 0.1f;
        moveSpeedBoost += 0.1f;
        zoneRadius += 1f;
        zoneDuration += 2f;
        cooldownTime = Mathf.Max(1f, cooldownTime - 0.5f);
    }

    private void InstantiateAccelerationZone()
    {
        Vector3 randomPos = GameManager.Instance._Player.transform.position + (Vector3)(Random.insideUnitCircle * randomPosOffset);
        randomPos.y = 5f; // y축은 5 고정
        GameObject zone = Instantiate(accelerationZonePrefab, randomPos, Quaternion.identity);
        zone.GetComponent<AccelerationZone>().Init(zoneRadius, zoneDuration, attackBoost, attackSpeedBoost, moveSpeedBoost);
    }
}
