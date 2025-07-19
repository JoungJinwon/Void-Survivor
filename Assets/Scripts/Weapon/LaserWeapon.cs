using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser Weapon", menuName = "My Scriptable Objects/Weapons/Laser Weapon")]
public class LaserWeapon : Weapon
{
    private float cooldownTime = 2.0f;

    [SerializeField] private float laserWidth = 1f;
    [SerializeField] private float laserRange = 100f;
    [SerializeField] private LineRenderer laserLinePrefab;
    [SerializeField] private float laserDuration = 0.5f;

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
            weaponDamage += 5;
            laserWidth *= 1.2f;
            Debug.Log($"Laser Weapon upgraded: Damage = {weaponDamage}, Range = {laserRange}");
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

        // Find nearest enemy in range
        player.targetEnemy = null;
        float minDist = float.MaxValue;
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < minDist && enemy.IsAlive)
            {
                minDist = dist;
                player.targetEnemy = enemy;
            }
        }

        if (player.targetEnemy != null)
        {
            Vector3 origin = player.transform.position;
            Vector3 directionToEnemy = (player.targetEnemy.transform.position - origin).normalized;

            // 다중 레이저 발사
            for (int i = 0; i < projectileCount; i++)
            {
                // 발사 위치에만 오프셋 적용, 방향은 동일하게 유지
                float offset = (i - (projectileCount - 1) / 2f) * 1f; // 1f는 레이저 간 위치 간격
                Vector3 rightVector = Vector3.Cross(directionToEnemy, Vector3.up).normalized;
                Vector3 laserOrigin = origin + rightVector * offset; // 시작 위치에만 오프셋 적용

                // 레이저는 항상 고정된 길이로 발사 (방향은 동일)
                Vector3 endPoint = laserOrigin + directionToEnemy * laserRange;
                
                // 경로상의 모든 적에게 데미지 적용
                RaycastHit[] hits = Physics.RaycastAll(laserOrigin, directionToEnemy, laserRange);
                foreach (RaycastHit hit in hits)
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null && enemy.IsAlive)
                    {
                        enemy.TakeDamage(weaponDamage);
                    }
                }

                // 레이저 시각 효과 (항상 고정된 길이)
                if (laserLinePrefab != null)
                    player.StartCoroutine(LaserEffect(laserOrigin, endPoint));
                else
                    Debug.LogWarning("Laser Line Renderer prefab is not assigned!");
            }

            lastAttackTime = Time.time;

            player.PlayAttackSound();

            Debug.Log($"Laser attack with {projectileCount} lasers!");
        }
        else
        {
            Debug.Log("Laser Weapon: No enemies in range to attack.");
        }
    }
    
    private IEnumerator LaserEffect(Vector3 origin, Vector3 endPoint)
    {
        LineRenderer lr = Instantiate(laserLinePrefab);
        lr.enabled = true;
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, endPoint);

        if (lr.material == null)
        {
            Debug.LogWarning("LineRenderer material is not assigned!");
        }
        
        yield return new WaitForSeconds(laserDuration);
        
        Destroy(lr.gameObject);
    }
}