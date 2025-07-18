using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/AccelerationZoneSkill")]
public class AccelerationZoneSkill : Skill
{
    public float attackBoost = 10f;
    public float attackSpeedBoost = 1.3f;
    public float moveSpeedBoost = 1.3f;

    public float zoneRadius = 20f;
    public float zoneDuration = 8f;

    public float spawnInterval = 7f;

    public GameObject accelerationZonePrefab;

    private float timer;

    public override void Activate()
    {
        base.Activate();
        
        timer = 0f;
        SkillManager.Instance.RegisterUpdate(UpdateSkill);
    }

    void UpdateSkill()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Vector3 randomPos = GameManager.Instance._Player.transform.position + (Vector3)(Random.insideUnitCircle * 5f);
            randomPos.y = 5f;
            GameObject zone = Instantiate(accelerationZonePrefab, randomPos, Quaternion.identity);
            zone.GetComponent<AccelerationZone>().Init(zoneRadius, zoneDuration, attackBoost, attackSpeedBoost, moveSpeedBoost);
        }
    }

    public override void Upgrade()
    {
        if (skillLevel >= maxLevel) return;

        skillLevel++;

        attackBoost += 5f;
        attackSpeedBoost += 0.1f;
        moveSpeedBoost += 0.1f;
        zoneRadius += 1f;
        zoneDuration += 2f;
        spawnInterval = Mathf.Max(1f, spawnInterval - 0.5f);
    }
}
