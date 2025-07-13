using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseHealthSkill")]
public class IncreaseHealthSkill : Skill
{
    public float healthIncreaseAmount = 30f;

    public override void Activate()
    {
        base.Activate();
        
        // 플레이어의 체력 증가
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        // 업그레이드 시 체력 증가량 추가
        GameManager.Instance._Player.IncreaseHealth(healthIncreaseAmount);
    }
}