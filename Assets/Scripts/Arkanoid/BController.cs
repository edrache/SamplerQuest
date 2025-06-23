using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float m_Speed = 10f;
    #endregion

    #region Private Fields
    private Rigidbody m_Rigidbody;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        LaunchBall();
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