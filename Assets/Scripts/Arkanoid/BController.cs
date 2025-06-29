using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_MaxSpeed = 15f; // Maximum allowed speed
    [Header("Rewired Control")]
    [SerializeField] private bool m_ControlByRewired = false;
    [SerializeField] private int m_PlayerId = 0;
    [SerializeField] private float m_BallControlForce = 10f;
    #endregion

    #region Private Fields
    private Rigidbody m_Rigidbody;
    private Rewired.Player m_Player;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        LaunchBall();
        if (m_ControlByRewired)
        {
            m_Player = Rewired.ReInput.players.GetPlayer(m_PlayerId);
        }
    }

    private void FixedUpdate()
    {
        if (m_ControlByRewired && m_Player != null)
        {
            float inputX = m_Player.GetAxis("BallHorizontal");
            float inputY = m_Player.GetAxis("BallVertical");
            Vector3 steer = new Vector3(inputX, inputY, 0f) * m_BallControlForce;
            m_Rigidbody.AddForce(steer, ForceMode.VelocityChange);
        }
        // Limit the ball's speed
        if (m_Rigidbody.velocity.magnitude > m_MaxSpeed)
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_MaxSpeed;
        }
    }
    #endregion

    #region Private Methods
    private void LaunchBall()
    {
        // Random direction in XY plane
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(randomDir.x, randomDir.y, 0f);
        m_Rigidbody.velocity = direction * m_Speed;
    }
    #endregion
}  