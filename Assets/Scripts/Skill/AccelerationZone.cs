using UnityEngine;

public class AccelerationZone : MonoBehaviour
{
    private bool isActivating = false;

    private float attackBoost;
    private float attackSpeedBoost;
    private float moveSpeedBoost;

    private float zoneRadius;
    private float zoneDuration;

    public void Init(float radius, float duration, float attack, float attackSpeed, float moveSpeed)
    {
        zoneRadius = radius;
        zoneDuration = duration;
        attackBoost = attack;
        attackSpeedBoost = attackSpeed;
        moveSpeedBoost = moveSpeed;

        // 존의 크기 설정
        transform.localScale = new Vector3(zoneRadius, 0.1f, zoneRadius);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;

        zoneDuration -= Time.deltaTime;
        if (zoneDuration <= 0)
        {
            if (isActivating)
                GameManager.Instance._Player.Decelerate(attackBoost, attackSpeedBoost, moveSpeedBoost);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                isActivating = true;
                player.Accelerate(attackBoost, attackSpeedBoost, moveSpeedBoost);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                isActivating = false;
                player.Decelerate(attackBoost, attackSpeedBoost, moveSpeedBoost);
            }
        }
    }
}
