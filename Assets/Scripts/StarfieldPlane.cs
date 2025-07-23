using UnityEngine;

[System.Serializable]
public class StarfieldSettings
{
    [Header("Star Properties")]
    [Range(10, 1000)]
    public float starDensity = 200f;
    
    [Range(0.0001f, 1f)]
    public float starSize = 0.1f;
    
    [Range(1, 5)]
    public int depthLayers = 3;
    
    [Range(0f, 1f)]
    public float bigStarChance = 0.1f;
    
    [Range(1f, 10f)]
    public float bigStarSizeMultiplier = 3f;
    
    [Header("Star Colors")]
    public Color starColor1 = Color.white;
    public Color starColor2 = new Color(0.8f, 0.9f, 1f, 1f);
    public Color starColor3 = new Color(1f, 0.8f, 0.6f, 1f);
    
    [Header("Animation")]
    [Range(0f, 10f)]
    public float twinkleSpeed = 2f;
}

[RequireComponent(typeof(MeshRenderer))]
public class StarfieldPlane : MonoBehaviour
{
    [Header("Starfield Configuration")]
    public StarfieldSettings starfieldSettings = new StarfieldSettings();
    
    private Material starfieldMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        // 매테리얼 복사 (인스턴스 생성)
        if (meshRenderer.material != null)
        {
            starfieldMaterial = new Material(meshRenderer.material);
            meshRenderer.material = starfieldMaterial;
        }
    }

    private void Start()
    {
        ApplySettings();
    }

    private void Update()
    {
        // 실시간으로 설정 적용 (에디터에서 테스트용)
        if (Application.isEditor)
        {
            ApplySettings();
        }
    }

    public void ApplySettings()
    {
        if (starfieldMaterial == null) return;

        starfieldMaterial.SetFloat("_StarDensity", starfieldSettings.starDensity);
        starfieldMaterial.SetFloat("_StarSize", starfieldSettings.starSize);
        starfieldMaterial.SetFloat("_DepthLayers", starfieldSettings.depthLayers);
        starfieldMaterial.SetFloat("_BigStarChance", starfieldSettings.bigStarChance);
        starfieldMaterial.SetFloat("_BigStarSizeMultiplier", starfieldSettings.bigStarSizeMultiplier);
        starfieldMaterial.SetColor("_StarColor1", starfieldSettings.starColor1);
        starfieldMaterial.SetColor("_StarColor2", starfieldSettings.starColor2);
        starfieldMaterial.SetColor("_StarColor3", starfieldSettings.starColor3);
        starfieldMaterial.SetFloat("_TwinkleSpeed", starfieldSettings.twinkleSpeed);
    }

    public void SetRandomVariation()
    {
        // 각 Plane마다 약간씩 다른 별 배치를 위한 랜덤 변화
        // density는 통일성을 위해 변경하지 않음
        starfieldSettings.starSize += Random.Range(-0.02f, 0.02f);
        starfieldSettings.bigStarChance += Random.Range(-0.03f, 0.03f);
        
        // 범위 제한
        starfieldSettings.starSize = Mathf.Clamp(starfieldSettings.starSize, 0.0001f, 1f);
        starfieldSettings.bigStarChance = Mathf.Clamp01(starfieldSettings.bigStarChance);
        
        ApplySettings();
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        if (starfieldMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(starfieldMaterial);
            else
                DestroyImmediate(starfieldMaterial);
        }
    }
}
