using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/BuffZoneSkill")]
public class BuffZoneSkill : Skill
{
    public GameObject buffZonePrefab;
    public float spawnInterval;

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
            Instantiate(buffZonePrefab, randomPos, Quaternion.identity);
        }
    }

    public override void Upgrade()
    {
        skillLevel++;
        spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.2f);
    }
}
