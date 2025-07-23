using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Scriptable Objects/Skills/Active/GuardianUnitSkill")]
public class GuardianUnitSkill : Skill
{
    private int unitCount = 0;
    public List<GameObject> guardianUnits = new List<GameObject>();

    public float unitAttackDamage;
    public float unitSpacing = 3f; // 유닛 간 간격
    public float playerAttachedDistance = 5f; // 플레이어로부터의 부착 거리

    public GameObject guardianPrefab;

    // 초기값 저장을 위한 변수
    [HideInInspector] public float initialUnitAttackDamage;

    public override void Activate()
    {
        base.Activate();

        Debug.Log($"Guardian Unit Skill Activate 진입");

        unitCount = 0;
        cooldownTime = 1.5f;

        // 리스트 초기화 보장
        if (guardianUnits == null)
            guardianUnits = new List<GameObject>();

        SkillManager.Instance.RegisterUpdate(UpdateSkill);
        
        InstantiateGuardianUnit();
    }

    public override void Upgrade()
    {
        base.Upgrade();

        // 유닛 스탯 상승
        unitAttackDamage += 2f;
        cooldownTime *= 0.9f;

        InstantiateGuardianUnit();
    }

    private void UpdateSkill()
    {
        if (unitCount > 0)
        {
            RepositionAllUnits();
        }
    }
    
    public override void StoreInitialValues()
    {
        base.StoreInitialValues();

        initialUnitAttackDamage = unitAttackDamage;
    }
    
    public override void ResetToInitialValues()
    {
        base.ResetToInitialValues();

        unitAttackDamage = initialUnitAttackDamage;

        // 기존 유닛들 제거
        for (int i = 0; i < guardianUnits.Count; i++)
        {
            if (guardianUnits[i] != null)
            {
                DestroyImmediate(guardianUnits[i]);
                guardianUnits[i] = null;
            }
        }
        
        guardianUnits.Clear();

        unitCount = 0;
    }

    private void InstantiateGuardianUnit()
    {
        if (guardianUnits == null)
        {
            Debug.Log("Guardian units list가 null이므로, 리스트를 생성합니다.");
            guardianUnits = new List<GameObject>();
        }

        // 임시 위치에 생성 (실제 위치는 RepositionAllUnits에서 설정)
        Vector3 playerPos = GameManager.Instance._Player.transform.position;
        guardianUnits.Add(Instantiate(guardianPrefab, playerPos, Quaternion.Euler(-40f, 180f, 0f)));
        
        Debug.Log($"Guardian unit {unitCount} instantiated");
        GuardianUnit guardianComponent = guardianUnits[unitCount].GetComponent<GuardianUnit>();
        
        if (guardianComponent != null)
        {
            guardianComponent.Init(unitAttackDamage, cooldownTime);
        }
        else
        {
            Debug.LogError($"Guardian unit {unitCount} does not have GuardianUnit component!");
        }
        
        unitCount++;
        
        // 모든 기존 유닛들을 재배치
        RepositionAllUnits();
    }

    // 공격 방향에 수직인 일렬횡대 포메이션 계산
    private void RepositionAllUnits()
    {
        Vector3 playerPos = GameManager.Instance._Player.transform.position;
        Vector3 attackDirection = GetAttackDirection(playerPos);
        Vector3 formationOrigin = playerPos + (-attackDirection * playerAttachedDistance);
        
        // 공격 방향에 수직인 방향 계산 (오른쪽 방향)
        Vector3 perpendicularDirection = Vector3.Cross(attackDirection, Vector3.up).normalized;
        
        for (int i = 0; i < unitCount; i++)
        {
            if (guardianUnits[i] != null)
            {
                GuardianUnit guardianComponent = guardianUnits[i].GetComponent<GuardianUnit>();
                
                if (guardianComponent != null)
                {
                    // 중앙을 기준으로 좌우 대칭 배치
                    float offset = (i - (unitCount - 1) / 2f) * unitSpacing;
                    Vector3 unitPosition = formationOrigin + perpendicularDirection * offset;
                    
                    guardianComponent.SetTargetPosition(unitPosition);
                }
            }
        }
    }
    
    private Vector3 GetAttackDirection(Vector3 playerPos)
    {
        Player player = GameManager.Instance._Player;
        
        // 플레이어가 타겟을 가지고 있는 경우
        if (player.targetEnemy != null && player.targetEnemy.IsAlive)
        {
            return (player.targetEnemy.transform.position - playerPos).normalized;
        }
        
        // 타겟이 없는 경우 가장 가까운 적을 찾아서 계산
        Enemy closestEnemy = FindClosestEnemy(playerPos);
        if (closestEnemy != null)
        {
            return (closestEnemy.transform.position - playerPos).normalized;
        }
        
        // 적이 없는 경우 앞쪽 방향
        return Vector3.forward;
    }
    
    private Enemy FindClosestEnemy(Vector3 playerPos)
    {
        float minDist = float.MaxValue;
        Enemy closestEnemy = null;
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                float dist = Vector3.Distance(playerPos, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = enemy;
                }
            }
        }
        
        return closestEnemy;
    }
}
