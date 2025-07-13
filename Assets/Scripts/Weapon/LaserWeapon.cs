using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser Weapon", menuName = "My Scriptable Objects/Weapons/Laser Weapon")]
public class LaserWeapon : Weapon
{
    private float cooldownTime = 2.0f;

    [SerializeField] private float laserWidth = 1f;
    [SerializeField] private float laserRange = 10f;
    [SerializeField] private LineRenderer laserLinePrefab;
    [SerializeField] private float laserDuration = 0.1f;

    public override void InitWeapon()
    {
        base.InitWeapon();
    }

    public override void UpgradeWeapon()
    {
        if (weaponLevel >= maxWeaponLevel)
        {
            Debug.LogWarning($"{this} Skill is already at MAXIMUM level!");
            return;
        }
        else
        {
            weaponLevel++;
            // 레이저 무기 업그레이드 로직 (예: 공격력 증가, 사거리 증가 등)
            attackDamage += 5f;
            laserWidth *= 1.2f;
            Debug.Log($"Laser Weapon upgraded: Damage = {attackDamage}, Range = {laserRange}");
        }
    }

    public override void IncreaseProjectileCount()
    {
        projectileCount++;
    }

    public override void TryAttack(Player player)
    {
        float playerAttackSpeed = player.GetPlayerAttackSpeed();
        float effectiveAttackTime = (Time.time - lastAttackTime) * attackIntervalMultiplier * playerAttackSpeed; // 스킬 경과 시간에 공격 속도를 적용
        if (effectiveAttackTime < cooldownTime) return;

        Vector3 origin = player.transform.position;
        Vector3 direction = player.transform.forward;
        
        // 다중 레이저 발사
        for (int i = 0; i < projectileCount; i++)
        {
            // 발사 방향을 기준으로 수직 방향으로 오프셋 계산
            float offset = (i - (projectileCount - 1) / 2f) * 0.3f; // 0.3f는 레이저 간 각도 간격
            Vector3 rightVector = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 offsetDirection = (direction + rightVector * offset).normalized;
            Vector3 laserOrigin = origin + rightVector * offset * 0.5f; // 시작 위치도 약간 오프셋

            RaycastHit hit;
            Vector3 endPoint = laserOrigin + offsetDirection * laserRange;
            if (Physics.Raycast(laserOrigin, offsetDirection, out hit, laserRange))
            {
                endPoint = hit.point;
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null && enemy.IsAlive)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }

            // 레이저 시각 효과
            if (laserLinePrefab != null)
            {
                LineRenderer lr = Instantiate(laserLinePrefab);
                lr.SetPosition(0, laserOrigin);
                lr.SetPosition(1, endPoint);
                Destroy(lr.gameObject, laserDuration);
            }
        }

        Debug.Log($"Laser attack with {projectileCount} lasers!");

        lastAttackTime = Time.time;
    }
}
