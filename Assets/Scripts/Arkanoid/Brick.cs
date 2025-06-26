using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private int m_HitPoints = 1;
    [SerializeField] private UnityEvent m_OnBrickDestroyed;
    [SerializeField] private UnityEvent m_OnHitPointsChanged;
    [SerializeField] private List<Material> m_Materials = new List<Material>();
    #endregion

    #region Private Fields
    private int m_CurrentHitPoints;
    private HashSet<GameObject> m_AlreadyHitThisFrame = new HashSet<GameObject>();
    private MeshRenderer m_MeshRenderer;
    private Material m_DefaultMaterial;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_MeshRenderer != null)
            m_DefaultMaterial = m_MeshRenderer.sharedMaterial;
        ResetHitPoints();
    }

    private void LateUpdate()
    {
        // Clear set at the end of each frame
        m_AlreadyHitThisFrame.Clear();
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter(Collision collision)
    {
        // Only react to BController
        if (collision.gameObject.GetComponent<BController>() == null)
            return;
        // Ignore multiple hits from the same ball in one frame
        if (m_AlreadyHitThisFrame.Contains(collision.gameObject))
            return;
        m_AlreadyHitThisFrame.Add(collision.gameObject);
        m_CurrentHitPoints--;
        UpdateMaterial();
        m_OnHitPointsChanged.Invoke();
        if (m_CurrentHitPoints <= 0)
        {
            m_OnBrickDestroyed.Invoke();
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Methods
    public void ResetHitPoints()
    {
        m_CurrentHitPoints = m_HitPoints;
        UpdateMaterial();
        m_OnHitPointsChanged.Invoke();
    }
    #endregion

    #region Private Methods
    private void UpdateMaterial()
    {
        if (m_MeshRenderer == null)
            return;
        int matIndex = Mathf.Clamp(m_CurrentHitPoints - 1, 0, m_Materials.Count - 1);
        if (m_Materials != null && m_Materials.Count > 0 && m_CurrentHitPoints > 0 && matIndex < m_Materials.Count)
        {
            m_MeshRenderer.material = m_Materials[matIndex];
        }
        else if (m_DefaultMaterial != null)
        {
            m_MeshRenderer.material = m_DefaultMaterial;
        }
    }
    #endregion

    #region Properties
    public int HitPoints => m_HitPoints;
    public int CurrentHitPoints => m_CurrentHitPoints;
    #endregion
} 