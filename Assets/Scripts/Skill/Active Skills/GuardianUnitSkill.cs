using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/GuardianUnitSkill")]
public class GuardianUnitSkill : Skill
{
    public float unitAttackDamage = 10f;

    public GameObject guardianPrefab;

    public override void Activate()
    {
        base.Activate();
        cooldownTime = 1f;

        InstantiateGuardianUnit();
    }

    public override void Upgrade()
    {
        skillLevel++;

        // 유닛 스탯 상승
        unitAttackDamage += 5f;
        cooldownTime *= 0.9f;

        // 유닛 수 증가
        InstantiateGuardianUnit();
    }

    private void InstantiateGuardianUnit()
    {
        GameObject guardianUnit = Instantiate(guardianPrefab, GameManager.Instance._Player.transform.position, Quaternion.identity);
        guardianUnit.GetComponent<GuardianUnit>().Init(unitAttackDamage, cooldownTime);
    }
}
