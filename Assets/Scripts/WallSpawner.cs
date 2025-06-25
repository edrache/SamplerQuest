using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WallSpawner : MonoBehaviour
{
    #region Inspector Fields
    [Header("Wall Settings")]
    [SerializeField] private GameObject m_WallPrefab;
    [SerializeField] private float m_Width = 10f;
    [SerializeField] private float m_Height = 5f;
    [SerializeField] private float m_Spacing = 0f;
    #endregion

    #region Public Properties
    public GameObject WallPrefab => m_WallPrefab;
    public float Width => m_Width;
    public float Height => m_Height;
    public float Spacing => m_Spacing;
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Spawn Walls")]
    public void SpawnWalls()
    {
        if (m_WallPrefab == null)
        {
            Debug.LogError("WallSpawner: Wall Prefab is not assigned.", this);
            return;
        }

        // Get prefab size (assume Renderer bounds, fallback to 1x1)
        Vector2 prefabSize = GetPrefabSize(m_WallPrefab);
        if (prefabSize.x <= 0f || prefabSize.y <= 0f)
        {
            Debug.LogError("WallSpawner: Could not determine prefab size.", this);
            return;
        }

        // Calculate positions for each edge
        // Bottom edge (left to right)
        SpawnEdge(Vector2.left * m_Width * 0.5f + Vector2.down * m_Height * 0.5f, Vector2.right, m_Width, prefabSize.x, prefabSize.y, true);
        // Top edge (left to right)
        SpawnEdge(Vector2.left * m_Width * 0.5f + Vector2.up * m_Height * 0.5f, Vector2.right, m_Width, prefabSize.x, prefabSize.y, true);
        // Left edge (bottom to top, bez rogów)
        SpawnEdge(Vector2.left * m_Width * 0.5f + Vector2.down * m_Height * 0.5f + Vector2.up * (prefabSize.y + m_Spacing), Vector2.up, m_Height - 2 * (prefabSize.y + m_Spacing), prefabSize.y, prefabSize.x, false);
        // Right edge (bottom to top, bez rogów)
        SpawnEdge(Vector2.right * m_Width * 0.5f + Vector2.down * m_Height * 0.5f + Vector2.up * (prefabSize.y + m_Spacing), Vector2.up, m_Height - 2 * (prefabSize.y + m_Spacing), prefabSize.y, prefabSize.x, false);
    }

    private void SpawnEdge(Vector2 start, Vector2 direction, float length, float stepSize, float otherStep, bool includeEnds)
    {
        int count = Mathf.Max(1, Mathf.FloorToInt((length - (includeEnds ? 0f : 2 * (stepSize + m_Spacing))) / (stepSize + m_Spacing)) + (includeEnds ? 1 : 0));
        for (int i = 0; i < count; i++)
        {
            // Skip corners for vertical edges
            if (!includeEnds && (i == 0 || i == count - 1))
                continue;
            Vector2 pos = start + direction * ((stepSize + m_Spacing) * i + (includeEnds ? 0f : 0f));
            Vector3 worldPos = new Vector3(pos.x, pos.y, 0f) + transform.position;
            GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(m_WallPrefab, this.transform);
            wall.transform.position = worldPos;
            wall.transform.rotation = Quaternion.identity;
        }
    }

    private Vector2 GetPrefabSize(GameObject prefab)
    {
        // Try to get Renderer bounds
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            return new Vector2(size.x, size.y);
        }
        // Fallback to 1x1
        return Vector2.one;
    }
#endif
} 