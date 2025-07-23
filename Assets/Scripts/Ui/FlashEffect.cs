using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    private static FlashEffect instance;
    public static FlashEffect Instance
    {
        get
        {
            if (instance == null)
            {
                CreateFlashEffect();
            }
            return instance;
        }
    }

    [SerializeField] private Image flashImage;
    private Canvas flashCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupFlashEffect();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static void CreateFlashEffect()
    {
        GameObject flashGO = new GameObject("FlashEffect");
        flashGO.AddComponent<FlashEffect>();
    }

    private void SetupFlashEffect()
    {
        // Canvas 생성
        GameObject canvasGO = new GameObject("FlashCanvas");
        canvasGO.transform.SetParent(transform);
        
        flashCanvas = canvasGO.AddComponent<Canvas>();
        flashCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        flashCanvas.sortingOrder = 999; // 가장 위에 렌더링
        
        // CanvasScaler 추가
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // 섬광 이미지 생성
        GameObject flashImageGO = new GameObject("FlashImage");
        flashImageGO.transform.SetParent(canvasGO.transform, false);
        
        flashImage = flashImageGO.AddComponent<Image>();
        flashImage.color = new Color(1f, 1f, 1f, 0f); // 투명한 흰색
        flashImage.raycastTarget = false; // UI 이벤트 차단 방지
        
        // 전체 화면을 덮도록 설정
        RectTransform rectTransform = flashImageGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    public void TriggerMassiveFlash()
    {
        StartCoroutine(FlashCoroutine(0.3f, Color.white));
    }

    public void TriggerFlash(float duration, Color color)
    {
        StartCoroutine(FlashCoroutine(duration, color));
    }

    private IEnumerator FlashCoroutine(float duration, Color flashColor)
    {
        if (flashImage == null) yield break;

        float elapsedTime = 0f;
        
        // 빠르게 밝게
        while (elapsedTime < duration * 0.1f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 0.8f, elapsedTime / (duration * 0.1f));
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }
        
        elapsedTime = 0f;
        
        // 천천히 어둡게
        while (elapsedTime < duration * 0.9f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.8f, 0f, elapsedTime / (duration * 0.9f));
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }
        
        // 완전히 투명하게
        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }
}
