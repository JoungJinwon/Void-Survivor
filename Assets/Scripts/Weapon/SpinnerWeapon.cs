using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Spinner Weapon", menuName = "My Scriptable Objects/Weapons/Spinner Weapon")]
public class SpinnerWeapon : Weapon
{
    private Transform spinnerOrigin;

    [SerializeField] private int spinnerCount = 2;
    [SerializeField] private float spinnerSize = 1f;
    [SerializeField] private float armRadius = 5f;

    private List<GameObject> equippedSpinners;

    [SerializeField] private GameObject spinnerPrefab;

    private void Start()
    {
        InitWeapon();
    }

    public override void InitWeapon()
    {
        base.InitWeapon();

        if (spinnerOrigin == null)
        {
            GameObject spinnerPivot = new GameObject("Spinner Pivot");
            if (GameManager.Instance._Player != null)
                spinnerPivot.transform.SetParent(GameManager.Instance._Player.transform);
            else
                Debug.LogWarning("Spinner Weapon: Player의 트랜스폼이 null이므로 참조할 수 없습니다!");
            spinnerOrigin = spinnerPivot.transform;
        }
    }

    public override void UpgradeWeapon()
    {
        InitWeapon();
        
        if (weaponLevel >= maxWeaponLevel)
        {
            Debug.LogWarning($"{this} Weapon is already at MAXIMUM level!");
            return;
        }
        else
        {
            weaponLevel++;
            IncreaseProjectileCount();
            spinnerSize *= 1.3f;
            armRadius *= 1.2f;
        }
    }

    public override void IncreaseProjectileCount()
    {
        spinnerCount++;
        
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

        for (int i = 0; i < spinnerCount; i++)
        {
            // 스피너를 플레이어 주변에 배치
            float angle = i * (360f / spinnerCount);
            Vector3 position = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * armRadius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * armRadius);
            position += Vector3.up; // 약간 위로 올려서 플레이어와 겹치지 않도록

            GameObject newSpinner = Instantiate(spinnerPrefab, position, Quaternion.identity);
            newSpinner.transform.SetParent(spinnerOrigin);
            equippedSpinners.Add(newSpinner);
        }
    }

    public override void TryAttack(Player player)
    {
        if (spinnerOrigin == null)
            spinnerOrigin = player.transform;

        float rotateSpeed = attackIntervalMultiplier * player.GetPlayerAttackSpeed();
        RotateSpinners(rotateSpeed);

        // 스피너 시각 효과
        if (spinnerPrefab != null)
        {
            GameObject spinner = Instantiate(spinnerPrefab, player.transform.position, Quaternion.identity);
            spinner.transform.SetParent(player.transform);
        }

        Debug.Log("Spinner attack!");

        lastAttackTime = Time.time;
    }

    public void RotateSpinners(float rotateSpeed)
    {
        if (spinnerOrigin == null) return;

        spinnerOrigin.Rotate(Vector3.up, 360f * Time.deltaTime * rotateSpeed);
    }
}
