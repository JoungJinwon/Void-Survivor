using System.Collections.Generic;
using UnityEngine;

public class StarfieldPlaneManager : MonoBehaviour
{
    [Header("Plane Settings")]
    public GameObject planePrefab;
    public float planeSize = 100f; // 각 Plane의 크기
    public float activationThreshold = 30f; // 플레이어가 이 거리에 도달하면 새 Plane 생성
    
    [Header("Grid Settings")]
    public int gridRadius = 2; // 플레이어 주변에 유지할 그리드 반경
    
    [Header("Unified Starfield Settings")]
    public float globalStarDensity = 200f; // 모든 Plane에 적용될 통일된 density
    
    private Dictionary<Vector2Int, GameObject> activePlanes = new Dictionary<Vector2Int, GameObject>();
    private Transform player;
    private Vector2Int currentPlayerGrid;
    private Vector2Int lastPlayerGrid;

    private void Start()
    {
        // 플레이어 참조 가져오기
        if (GameManager.Instance != null && GameManager.Instance._Player != null)
        {
            player = GameManager.Instance._Player.transform;
        }
        else
        {
            Debug.LogError("Player not found! StarfieldPlaneManager requires a player reference.");
            return;
        }

        // 초기 Plane 생성
        InitializeInitialPlanes();
    }

    private void Update()
    {
        if (player == null) return;

        // 플레이어의 현재 그리드 위치 계산
        currentPlayerGrid = WorldToGrid(player.position);

        // 플레이어가 다른 그리드로 이동했을 때만 업데이트
        if (currentPlayerGrid != lastPlayerGrid)
        {
            UpdatePlanes();
            lastPlayerGrid = currentPlayerGrid;
        }
    }

    private void InitializeInitialPlanes()
    {
        // 플레이어 주변에 초기 Plane들 생성
        Vector2Int playerGrid = WorldToGrid(player.position);
        
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int z = -gridRadius; z <= gridRadius; z++)
            {
                Vector2Int gridPos = playerGrid + new Vector2Int(x, z);
                CreatePlaneAtGrid(gridPos);
            }
        }

        lastPlayerGrid = playerGrid;
    }

    private void UpdatePlanes()
    {
        // 플레이어 주변 필요한 위치에 Plane 생성
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int z = -gridRadius; z <= gridRadius; z++)
            {
                Vector2Int gridPos = currentPlayerGrid + new Vector2Int(x, z);
                
                if (!activePlanes.ContainsKey(gridPos))
                {
                    CreatePlaneAtGrid(gridPos);
                }
            }
        }

        // 너무 멀리 떨어진 Plane들 제거
        List<Vector2Int> planesToRemove = new List<Vector2Int>();
        
        foreach (var kvp in activePlanes)
        {
            Vector2Int gridPos = kvp.Key;
            float distance = Vector2Int.Distance(gridPos, currentPlayerGrid);
            
            if (distance > gridRadius + 1) // 여유분 추가
            {
                planesToRemove.Add(gridPos);
            }
        }

        // 제거할 Plane들 삭제
        foreach (Vector2Int gridPos in planesToRemove)
        {
            RemovePlaneAtGrid(gridPos);
        }
    }

    private void CreatePlaneAtGrid(Vector2Int gridPos)
    {
        if (activePlanes.ContainsKey(gridPos)) return;

        Vector3 worldPos = GridToWorld(gridPos);
        GameObject newPlane = Instantiate(planePrefab, worldPos, Quaternion.identity, transform);
        
        // Plane 이름 설정
        newPlane.name = $"StarfieldPlane_{gridPos.x}_{gridPos.y}";
        
        // Plane 크기 설정
        newPlane.transform.localScale = Vector3.one * (planeSize / 10f); // Unity 기본 Plane은 10x10 크기
        
        // StarfieldPlane 컴포넌트가 있다면 통일된 density 적용하고 약간의 변화만 적용
        var starfieldPlane = newPlane.GetComponent<StarfieldPlane>();
        if (starfieldPlane != null)
        {
            // 통일된 density 적용
            starfieldPlane.starfieldSettings.starDensity = globalStarDensity;
        }
        
        activePlanes[gridPos] = newPlane;
    }

    private void RemovePlaneAtGrid(Vector2Int gridPos)
    {
        if (activePlanes.TryGetValue(gridPos, out GameObject plane))
        {
            Destroy(plane);
            activePlanes.Remove(gridPos);
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gridX = Mathf.FloorToInt(worldPos.x / planeSize);
        int gridZ = Mathf.FloorToInt(worldPos.z / planeSize);
        return new Vector2Int(gridX, gridZ);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        float worldX = gridPos.x * planeSize + planeSize * 0.5f;
        float worldZ = gridPos.y * planeSize + planeSize * 0.5f;
        return new Vector3(worldX, 0f, worldZ);
    }

    // 모든 기존 Plane에 새로운 density 적용 (런타임에서 호출 가능)
    public void UpdateAllPlanesSettings()
    {
        foreach (var kvp in activePlanes)
        {
            var starfieldPlane = kvp.Value.GetComponent<StarfieldPlane>();
            if (starfieldPlane != null)
            {
                starfieldPlane.starfieldSettings.starDensity = globalStarDensity;
                starfieldPlane.ApplySettings();
            }
        }
    }

    // 디버그용 Gizmos
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Vector2Int playerGrid = WorldToGrid(player.position);
        
        // 현재 플레이어 그리드 표시
        Vector3 playerGridWorld = GridToWorld(playerGrid);
        Gizmos.DrawWireCube(playerGridWorld, Vector3.one * planeSize);

        // 활성 그리드 영역 표시
        Gizmos.color = Color.green;
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int z = -gridRadius; z <= gridRadius; z++)
            {
                Vector2Int gridPos = playerGrid + new Vector2Int(x, z);
                Vector3 gridWorld = GridToWorld(gridPos);
                Gizmos.DrawWireCube(gridWorld, Vector3.one * planeSize * 0.9f);
            }
        }
    }
}