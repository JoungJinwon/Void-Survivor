using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Passive/IncreaseMoveSpeedSkill")]
public class IncreaseMoveSpeedSkill : Skill
{
    public float moveSpeedIncreaseAmount;

    public override void Activate()
    {
        base.Activate();
        
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }

    public override void Upgrade()
    {
        skillLevel++;
        GameManager.Instance._Player.IncreaseMoveSpeed(moveSpeedIncreaseAmount);
    }
}