using UnityEngine;
using Rewired;

public class TimeScaleSmoothSwitcher : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int m_PlayerId = 0;
    [SerializeField] private string m_ActionName = "SlowMotion";
    [SerializeField] private float m_TargetTimeScale = 0.5f;
    [SerializeField] private float m_SmoothSpeed = 5f;
    [Header("Rewired Map Category")]
    [SerializeField] private RewiredMapCategorySwitcher m_MapCategorySwitcher;
    [SerializeField] private int m_SlowMotionCategoryIndex = 1;
    #endregion

    #region Private Fields
    private Player m_Player;
    private float m_DesiredTimeScale = 1f;
    private int m_PreviousCategoryIndex = 0;
    private bool m_IsSlowMotionActive = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
        m_DesiredTimeScale = 1f;
    }

    private void Update()
    {
        if (m_Player == null) return;

        bool slowMotionActive = m_Player.GetButton(m_ActionName);

        if (slowMotionActive && !m_IsSlowMotionActive)
        {
            // Zapamiętaj poprzednią kategorię i przełącz na slow motion
            if (m_MapCategorySwitcher != null)
            {
                m_PreviousCategoryIndex = m_MapCategorySwitcher.m_MapCategoryIndex;
                m_MapCategorySwitcher.SetActiveMapCategory(m_SlowMotionCategoryIndex);
            }
            m_IsSlowMotionActive = true;
        }
        else if (!slowMotionActive && m_IsSlowMotionActive)
        {
            // Przywróć poprzednią kategorię
            if (m_MapCategorySwitcher != null)
            {
                m_MapCategorySwitcher.SetActiveMapCategory(m_PreviousCategoryIndex);
            }
            m_IsSlowMotionActive = false;
        }

        m_DesiredTimeScale = slowMotionActive ? m_TargetTimeScale : 1f;

        // Smoothly interpolate time scale
        Time.timeScale = Mathf.Lerp(Time.timeScale, m_DesiredTimeScale, m_SmoothSpeed * Time.unscaledDeltaTime);
        if (Mathf.Abs(Time.timeScale - m_DesiredTimeScale) < 0.01f)
        {
            Time.timeScale = m_DesiredTimeScale;
        }
    }
    #endregion
} 