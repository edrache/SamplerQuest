using UnityEngine;
using Rewired;
using Unity.Cinemachine;

public class CinemachinePrioritySwitcher : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int m_PlayerId = 0;
    [SerializeField] private string m_ActionName = "CameraSwitch";
    [SerializeField] private CinemachineCamera m_TargetCamera;
    [SerializeField] private int m_OverridePriority = 20;
    #endregion

    #region Private Fields
    private Player m_Player;
    private int m_PreviousPriority;
    private bool m_IsOverridden = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
        if (m_TargetCamera != null)
        {
            m_PreviousPriority = m_TargetCamera.Priority;
        }
    }

    private void Update()
    {
        if (m_TargetCamera == null || m_Player == null)
            return;

        if (m_Player.GetButton(m_ActionName))
        {
            if (!m_IsOverridden)
            {
                m_PreviousPriority = m_TargetCamera.Priority;
                m_TargetCamera.Priority = m_OverridePriority;
                m_IsOverridden = true;
            }
        }
        else
        {
            if (m_IsOverridden)
            {
                m_TargetCamera.Priority = m_PreviousPriority;
                m_IsOverridden = false;
            }
        }
    }
    #endregion
} 