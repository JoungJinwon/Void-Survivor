using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Transform target;
    private Camera cam;
    private float maxDuration = 5f;
    private float visibleUntil;
    private bool isActive;

    public void Initialize(Transform worldTarget, Camera camera)
    {
        target = worldTarget;
        cam = camera;
        gameObject.SetActive(true);
        isActive = true;
        visibleUntil = Time.time + maxDuration;
    }

    public void UpdateHealth(float normalizedValue)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
        visibleUntil = Time.time + maxDuration;

        if (!isActive)
        {
            gameObject.SetActive(true);
            isActive = true;
        }

        Tick();
    }

    public void Tick()
    {
        if (!isActive) return;

        if (target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position);
            RectTransform canvasRect = transform.parent as RectTransform;
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, cam, out localPoint))
            {
                // UI의 로컬 좌표로 변환하여 위치 지정
                (transform as RectTransform).localPosition = localPoint + Vector2.down * 2;
            }
        }

        if (Time.time > visibleUntil)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        isActive = false;
        HealthBarManager.Instance.ReturnToPool(this);
    }
}