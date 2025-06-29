using UnityEngine;
using Rewired;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class BallImpulseOnAction : MonoBehaviour
{
    #region Serialized Fields
    [Header("Rewired Settings")]
    [SerializeField] private int m_PlayerId = 0;
    [SerializeField] private string m_ActionName = "Hit";
    [Header("Impulse Settings")]
    [SerializeField] private float m_ImpulseSpeed = 15f;
    [SerializeField] private float m_ImpulseWindow = 0.2f; // seconds before/after collision
    [SerializeField] private bool m_UseBallVelocityDirection = true; // Use current ball velocity direction for impulse
    [SerializeField] private Vector3 m_CustomDirection = Vector3.up;
    [Header("Tag Settings")]
    [SerializeField] private string m_BallTag = "Ball";
    [SerializeField] private MMF_Player m_ImpulseFeedback;
    [SerializeField] private bool m_ReverseDirection = false; // Reverse the direction of the impulse
    #endregion

    #region Events
    [Header("Events")]
    public UnityEvent OnImpulseGiven;
    #endregion

    #region Private Fields
    private Player m_Player;
    private bool m_CanImpulse = false;
    private float m_CollisionTime = 0f;
    private Rigidbody m_BallRigidbody = null;
    private Vector3 m_LastCollisionNormal = Vector3.up;
    private bool m_ImpulseGiven = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
    }

    private void Update()
    {
        if (!m_CanImpulse || m_ImpulseGiven || m_BallRigidbody == null)
            return;

        float timeSinceCollision = Time.time - m_CollisionTime;
        if (Mathf.Abs(timeSinceCollision) > m_ImpulseWindow)
        {
            m_CanImpulse = false;
            m_BallRigidbody = null;
            return;
        }

        if (m_Player.GetButtonDown(m_ActionName))
        {
            Vector3 direction;
            if (m_UseBallVelocityDirection && m_BallRigidbody != null)
            {
                direction = m_BallRigidbody.velocity.normalized;
            }
            else
            {
                direction = m_CustomDirection.normalized;
            }
            if (m_ReverseDirection)
            {
                direction = -direction;
            }
            m_BallRigidbody.velocity = direction * m_ImpulseSpeed;
            m_ImpulseGiven = true;
            OnImpulseGiven?.Invoke();
            if (m_ImpulseFeedback != null)
            {
                m_ImpulseFeedback.PlayFeedbacks();
            }
            m_CanImpulse = false;
            m_BallRigidbody = null;
        }
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter(Collision collision)
    {
        if (m_ImpulseGiven)
            return;
        if (!collision.gameObject.CompareTag(m_BallTag))
            return;
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            return;
        m_BallRigidbody = rb;
        m_CollisionTime = Time.time;
        m_CanImpulse = true;
        m_ImpulseGiven = false;
        // Use the first contact normal for direction
        if (collision.contactCount > 0)
            m_LastCollisionNormal = collision.GetContact(0).normal;
        else
            m_LastCollisionNormal = (collision.transform.position - transform.position).normalized;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(m_BallTag))
        {
            m_ImpulseGiven = false;
        }
    }
    #endregion

    #region Public Methods
    public void ResetImpulse()
    {
        m_ImpulseGiven = false;
        m_CanImpulse = false;
        m_BallRigidbody = null;
    }
    #endregion
} 