using UnityEngine;

public class VelocityBooster : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private float m_BoostStrength = 5f;
    #endregion

    #region Unity Lifecycle
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has BController
        BController ballController = collision.gameObject.GetComponent<BController>();
        if (ballController == null)
            return;

        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
        if (ballRb == null)
            return;

        // Use the first contact normal (direction of bounce)
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            // Project current velocity onto normal
            float velocityAlongNormal = Vector3.Dot(ballRb.velocity, normal);
            // Add boost only in the direction of the normal
            float newVelocityAlongNormal = velocityAlongNormal + m_BoostStrength;
            Vector3 velocityParallel = ballRb.velocity - normal * velocityAlongNormal;
            ballRb.velocity = velocityParallel + normal * newVelocityAlongNormal;
        }
    }
    #endregion
} 