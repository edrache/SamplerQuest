using UnityEngine;
using Rewired;
using MoreMountains.Feedbacks;

public class PaddleController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int m_PlayerId = 0; // Rewired player ID
    [SerializeField] private float m_MoveSpeed = 10f;
    [SerializeField] private float m_SpringStrength = 200f; // spring constant (k)
    [SerializeField] private float m_Damping = 20f;         // damping (d)
    [SerializeField] private float m_RotateInputStrength = 300f; // input torque
    [SerializeField] private MMF_Player m_CameraShakePlayer; // Camera shake feedback
    [SerializeField] private float m_MoveForce = 20f; // force applied for movement
    [SerializeField] private float m_MaxSpeed = 10f;  // max speed
    [SerializeField] private float m_Friction = 5f;   // friction (drag)
    #endregion

    #region Private Fields
    private Player m_Player;
    private float m_CurrentVelocityZ = 0f; // for smooth damp
    private float m_AngleZ = 0f;
    private float m_AngularVelocity = 0f;
    private Rigidbody m_Rigidbody;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
        m_Rigidbody.drag = m_Friction;
        m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        float moveX = m_Player.GetAxis("Move Horizontal");
        Vector3 velocity = m_Rigidbody.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, moveX * m_MaxSpeed, m_MoveForce * Time.deltaTime);
        m_Rigidbody.velocity = new Vector3(velocity.x, 0f, 0f);

        // Spring-based rotation on Z axis
        float rotateInput = m_Player.GetAxis("Rotate"); // -1..1
        float torque = -m_SpringStrength * m_AngleZ - m_Damping * m_AngularVelocity + m_RotateInputStrength * rotateInput;
        m_AngularVelocity += torque * Time.deltaTime;
        m_AngleZ += m_AngularVelocity * Time.deltaTime;
        m_AngleZ = Mathf.Clamp(m_AngleZ, -45f, 45f);
        Vector3 euler = transform.eulerAngles;
        euler.z = m_AngleZ;
        transform.eulerAngles = euler;
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the ball (BController)
        if (collision.gameObject.GetComponent<BController>() != null)
        {
            m_CameraShakePlayer?.PlayFeedbacks();
        }
    }
    #endregion
} 