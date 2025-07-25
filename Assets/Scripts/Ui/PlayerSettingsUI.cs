using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsUI : MonoBehaviour
{
    private const float WEAPON_SELECT_ANIM_DURATION = 0.1f;

    private float prevSelectionX;
    private float selctionX;
    private float bulletWeaponRectX;
    private float laserWeaponRectX;
    private float spinnerWeaponRectX;

    [Header("Buttons")]
    public Button bulletWeaponButton;
    public Button laserWeaponButton;
    public Button spinnerWeaponButton;

    [Header("Weapon Selection")]
    public RectTransform weaponSeletionRect;
    public Image weaponSelectionBox;

    [Header("Player Stats Text")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackDamageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI moveSpeedText;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (SettingsManager.Instance == null || SettingsManager.Instance.playerSettings == null)
        {
            Debug.LogError("SettingsManager or PlayerSettings not found!");
            return;
        }

        var settings = SettingsManager.Instance.playerSettings;

        if (bulletWeaponButton != null && laserWeaponButton != null && spinnerWeaponButton != null)
        {
            bulletWeaponButton.onClick.AddListener(() => OnBulletWeaponSelected());
            laserWeaponButton.onClick.AddListener(() => OnLaserWeaponSelected());
            spinnerWeaponButton.onClick.AddListener(() => OnSpinnerWeaponSelected());
        }
        else
        {
            Debug.LogError("Weapon buttons are not assigned in the PlayerSettingsUI.");
        }

        bulletWeaponRectX = bulletWeaponButton.GetComponent<RectTransform>().anchoredPosition.x;
        laserWeaponRectX = laserWeaponButton.GetComponent<RectTransform>().anchoredPosition.x;
        spinnerWeaponRectX = spinnerWeaponButton.GetComponent<RectTransform>().anchoredPosition.x;

        if (weaponSelectionBox != null)
        {
            weaponSeletionRect = weaponSelectionBox.GetComponent<RectTransform>();

            weaponSeletionRect.anchoredPosition = settings.playerWeapon switch
            {
                BulletWeapon _ => new Vector2(bulletWeaponRectX, weaponSeletionRect.anchoredPosition.y),
                LaserWeapon _ => new Vector2(laserWeaponRectX, weaponSeletionRect.anchoredPosition.y),
                SpinnerWeapon _ => new Vector2(spinnerWeaponRectX, weaponSeletionRect.anchoredPosition.y),
                _ => weaponSeletionRect.anchoredPosition // Default position if no match
            };

            prevSelectionX = weaponSeletionRect.anchoredPosition.x;
        }

        UpdateStatsText();
    }

    private void OnBulletWeaponSelected()
    {
        Weapon weapon = bulletWeaponButton.GetComponent<WeaponSlot>()?.equippedWeapon;
        SettingsManager.Instance.SetPlayerWeapon(weapon);
        selctionX = bulletWeaponRectX;
        StartCoroutine(UpdateWeaponSelection());
    }

    private void OnLaserWeaponSelected()
    {
        Weapon weapon = laserWeaponButton.GetComponent<WeaponSlot>()?.equippedWeapon;
        SettingsManager.Instance.SetPlayerWeapon(weapon);
        selctionX = laserWeaponRectX;
        StartCoroutine(UpdateWeaponSelection());
    }

    private void OnSpinnerWeaponSelected()
    {
        Weapon weapon = spinnerWeaponButton.GetComponent<WeaponSlot>()?.equippedWeapon;
        SettingsManager.Instance.SetPlayerWeapon(weapon);
        selctionX = spinnerWeaponRectX;
        StartCoroutine(UpdateWeaponSelection());
    }

    private IEnumerator UpdateWeaponSelection()
    {
        float elapsedTime = 0f;

        while (elapsedTime < WEAPON_SELECT_ANIM_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / WEAPON_SELECT_ANIM_DURATION);
            weaponSeletionRect.anchoredPosition = new Vector2(
                Mathf.Lerp(prevSelectionX, selctionX, t),
                weaponSeletionRect.anchoredPosition.y
            );
            yield return null;
        }

        prevSelectionX = selctionX;
    }

    private void UpdateStatsText()
    {
        var settings = SettingsManager.Instance.playerSettings;

        if (hpText != null)
            hpText.text = $"{settings.baseMaxHealth}";

        if (attackDamageText != null)
            attackDamageText.text = $"{settings.baseAttackDamage}";

        if (attackSpeedText != null)
            attackSpeedText.text = $"{settings.baseAttackSpeed}";

        if (moveSpeedText != null)
            moveSpeedText.text = $"{settings.baseMoveSpeed}";
    }
}
