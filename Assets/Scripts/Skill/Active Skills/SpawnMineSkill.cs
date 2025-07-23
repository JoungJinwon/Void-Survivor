using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/SpawnMineSkill")]
public class SpawnMineSkill : Skill
{
    private float timer;

    public float mineDamage;
    public float mineExplosionRadius;

    public GameObject minePrefab;
    
    // 초기값 저장을 위한 변수들
    [HideInInspector] public float initialMineDamage;
    [HideInInspector] public float initialMineExplosionRadius;

    public override void Activate()
    {
        base.Activate();

        timer = 0f;
        initialCooldownTime = 5f;
        
        SkillManager.Instance.RegisterUpdate(UpdateSkill);
    }

    void UpdateSkill()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTime)
        {
            timer = 0f;
            InstantiateMine();
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();

        cooldownTime -= 0.5f;
        mineDamage += 5f;
        mineExplosionRadius += 3f;
    }

    private void InstantiateMine()
    {
        GameObject mine = Instantiate(minePrefab, GameManager.Instance._Player.transform.position, Quaternion.identity);
        mine.GetComponent<Mine>().InitMine(mineDamage, mineExplosionRadius);
    }
    
    public override void StoreInitialValues()
    {
        if (hasStoredInitialValues) return;

        base.StoreInitialValues();

        initialMineDamage = mineDamage;
        initialMineExplosionRadius = mineExplosionRadius;
    }
    
    public override void ResetToInitialValues()
    {
        base.ResetToInitialValues();

        mineDamage = initialMineDamage;
        mineExplosionRadius = initialMineExplosionRadius;
    }
}
