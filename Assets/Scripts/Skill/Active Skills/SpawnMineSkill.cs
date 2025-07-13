using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/SpawnMineSkill")]
public class SpawnMineSkill : Skill
{
    public GameObject minePrefab;
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
            Instantiate(minePrefab, GameManager.Instance._Player.transform.position, Quaternion.identity);
        }
    }

    public override void Upgrade()
    {
        skillLevel++;
        // 예: spawnInterval 감소 등
        spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.2f);
    }
}
