using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/GuardianUnitSkill")]
public class GuardianUnitSkill : Skill
{
    public GameObject guardianPrefab;

    public override void Activate()
    {
        base.Activate();
        
        Instantiate(guardianPrefab, GameManager.Instance._Player.transform.position, Quaternion.identity);
    }

    public override void Upgrade()
    {
        skillLevel++;

        // 예: 추가 수호자 소환 등
        Instantiate(guardianPrefab, GameManager.Instance._Player.transform.position, Quaternion.identity);
    }
}
