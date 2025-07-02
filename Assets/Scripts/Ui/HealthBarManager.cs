using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRoot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private int initialPoolSize = 50;

    private Queue<HealthBar> healthBarPool = new Queue<HealthBar>();
    private List<HealthBar> activeBars = new List<HealthBar>();

    public static HealthBarManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        InitializePool();
    }

    private void Update()
    {
        foreach (var bar in activeBars.ToArray())
        {
            bar.Tick();
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(healthBarPrefab, canvasRoot);
            obj.SetActive(false);
            healthBarPool.Enqueue(obj.GetComponent<HealthBar>());
        }
    }

    public HealthBar RequestBar(Transform target)
    {
        HealthBar bar = healthBarPool.Count > 0 ? healthBarPool.Dequeue() : Instantiate(healthBarPrefab, canvasRoot).GetComponent<HealthBar>();
        
        bar.Initialize(target, mainCamera);
        activeBars.Add(bar);
        return bar;
    }

    public void ReturnToPool(HealthBar bar)
    {
        if (activeBars.Contains(bar)) activeBars.Remove(bar);
        healthBarPool.Enqueue(bar);
    }
}
