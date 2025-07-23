using UnityEngine;

[CreateAssetMenu(fileName = "StarfieldConfig", menuName = "My Scriptable Objects/Starfield Configuration")]
public class StarfieldConfiguration : ScriptableObject
{
    [Header("Plane Management")]
    public GameObject starfieldPlanePrefab;
    public float planeSize = 100f;
    public float activationThreshold = 30f;
    public int gridRadius = 2;
    
    [Header("Global Starfield Settings")]
    public StarfieldSettings globalStarfieldSettings = new StarfieldSettings();
    
    [Header("Variation Settings")]
    [Range(0f, 1f)]
    public float variationAmount = 0.1f;
    
    public bool enableRandomVariation = true;
}
