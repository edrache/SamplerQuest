using UnityEngine;
using Rewired;

public class PaddleController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int m_PlayerId = 0; // Rewired player ID
    [SerializeField] private float m_MoveSpeed = 10f;
    [SerializeField] private float m_SpringStrength = 200f; // spring constant (k)
    [SerializeField] private float m_Damping = 20f;         // damping (d)
    [SerializeField] private float m_RotateInputStrength = 300f; // input torque
    #endregion

    #region Private Fields
    private Player m_Player;
    private float m_CurrentVelocityZ = 0f; // for smooth damp
    private float m_AngleZ = 0f;
    private float m_AngularVelocity = 0f;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
    }

    private void Update()
    {
        float moveX = m_Player.GetAxis("Move Horizontal"); // configured in Rewired
        Vector3 position = transform.position;
        position.x += moveX * m_MoveSpeed * Time.deltaTime;
        transform.position = position;

        // Spring-based rotation on Z axis
        float rotateInput = m_Player.GetAxis("Rotate"); // -1..1
        // Spring physics: torque = -k * angle - d * angularVelocity + input
        float torque = -m_SpringStrength * m_AngleZ - m_Damping * m_AngularVelocity + m_RotateInputStrength * rotateInput;
        m_AngularVelocity += torque * Time.deltaTime;
        m_AngleZ += m_AngularVelocity * Time.deltaTime;
        // Clamp angle (optional, e.g. +/- 45 degrees)
        m_AngleZ = Mathf.Clamp(m_AngleZ, -45f, 45f);
        // Apply rotation
        Vector3 euler = transform.eulerAngles;
        euler.z = m_AngleZ;
        transform.eulerAngles = euler;
    }
    #endregion
} 