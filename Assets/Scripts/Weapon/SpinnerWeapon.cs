using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Spinner Weapon", menuName = "My Scriptable Objects/Weapons/Spinner Weapon")]
public class SpinnerWeapon : Weapon
{
    private Transform spinnerOrigin;
    
    [SerializeField] private float spinnerSize = 2f;
    [SerializeField] private float armRadius = 10f;

    private List<GameObject> equippedSpinners;

    [SerializeField] private GameObject spinnerPrefab;

    public override void InitWeapon()
    {
        base.InitWeapon();

        if (spinnerOrigin == null)
        {
            GameObject spinnerPivot = new GameObject("Spinner Pivot");
            if (GameManager.Instance._Player != null)
            {
                spinnerPivot.transform.SetParent(GameManager.Instance._Player.transform);
                spinnerPivot.transform.localPosition = Vector3.zero; // 플레이어의 위치에 맞춤
            }
            else
                Debug.LogWarning("Spinner Weapon: Player의 트랜스폼이 null이므로 참조할 수 없습니다!");
            spinnerOrigin = spinnerPivot.transform;
        }

        projectileCount = 2;
        attackIntervalMultiplier = 0.4f;

        CreateSpinners();
    }

    public override void UpgradeWeapon()
    {
        if (weaponLevel >= maxWeaponLevel)
        {
            Debug.LogWarning($"{this} Weapon is already at MAXIMUM level!");
            return;
        }
        else
        {
            weaponLevel++;
            spinnerSize *= 1.3f;
            armRadius *= 1.2f;
            weaponDamage += 5;
            attackIntervalMultiplier *= 1.2f;
            IncreaseProjectileCount();
        }
    }

    public override void IncreaseProjectileCount()
    {
        projectileCount++;
        CreateSpinners();
    }

    private void CreateSpinners()
    {
        if (equippedSpinners == null)
            equippedSpinners = new List<GameObject>();
        else
        {
            foreach (GameObject spinner in equippedSpinners)
            {
                if (spinner != null)
                    Destroy(spinner);
            }

            equippedSpinners.Clear();
        }

        for (int i = 0; i < projectileCount; i++)
        {
            // 스피너를 플레이어 주변에 배치
            float angle = i * (360f / projectileCount);
            Vector3 localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * armRadius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * armRadius);

            GameObject newSpinner = Instantiate(spinnerPrefab, spinnerOrigin.position, Quaternion.Euler(-90f, 0f, 0f)); // 스피너 모델을 -90도 회전시켜 위를 향하게 한다
            newSpinner.transform.SetParent(spinnerOrigin);
            newSpinner.transform.localPosition = localPosition;
            newSpinner.transform.localScale *= spinnerSize; // 스피너 크기 조정
            equippedSpinners.Add(newSpinner);
            newSpinner.GetComponent<Spinner>().Init((int)weaponDamage);
        }
    }

    public override void TryAttack(Player player)
    {
        if (spinnerOrigin == null)
            spinnerOrigin = player.transform;

        float rotateSpeed = attackIntervalMultiplier * player.GetPlayerAttackSpeed();
        RotateSpinners(rotateSpeed);
    }

    public void RotateSpinners(float rotateSpeed)
    {
        if (spinnerOrigin == null) return;

        spinnerOrigin.Rotate(Vector3.up, 360f * Time.deltaTime * rotateSpeed);
    }
}
