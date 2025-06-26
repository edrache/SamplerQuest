using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridSpawner : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private int m_Columns = 5;
    [SerializeField] private int m_Rows = 3;
    [SerializeField] private float m_Spacing = 0.2f;
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Spawn Grid")]
    public void SpawnGrid()
    {
        if (m_Prefab == null)
        {
            Debug.LogError("GridSpawner: Prefab is not assigned.", this);
            return;
        }
        if (m_Columns < 1 || m_Rows < 1)
        {
            Debug.LogError("GridSpawner: Columns and Rows must be at least 1.", this);
            return;
        }

        // Get prefab size (Renderer bounds, fallback to 1x1)
        Vector2 prefabSize = GetPrefabSize(m_Prefab);
        float cellWidth = prefabSize.x + m_Spacing;
        float cellHeight = prefabSize.y + m_Spacing;
        float totalWidth = cellWidth * m_Columns - m_Spacing;
        float totalHeight = cellHeight * m_Rows - m_Spacing;
        Vector2 origin = new Vector2(-totalWidth * 0.5f + cellWidth * 0.5f, -totalHeight * 0.5f + cellHeight * 0.5f);

        for (int row = 0; row < m_Rows; row++)
        {
            for (int col = 0; col < m_Columns; col++)
            {
                Vector2 offset = new Vector2(col * cellWidth, row * cellHeight);
                Vector2 pos = origin + offset;
                Vector3 worldPos = new Vector3(pos.x, pos.y, 0f) + transform.position;
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(m_Prefab, this.transform);
                obj.transform.position = worldPos;
                obj.transform.rotation = Quaternion.identity;
            }
        }
    }

    private Vector2 GetPrefabSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            return new Vector2(size.x, size.y);
        }
        return Vector2.one;
    }
#endif
} 