using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    private float bounceForce = 1f;
    public float minBounceVelocity = 1f;
    public float maxBounceVelocity = 20f;
    private float bounceAcceleration = 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Check the normal of the first contact point
            Vector2 contactNormal = collision.contacts[0].normal;

            // Only apply bounce if the collision is from the top
            if (Vector2.Dot(contactNormal, Vector2.up) < 0.9f)  // Close to (0, 1)
            {
                float impactSpeed = Mathf.Abs(collision.relativeVelocity.y);

                float rawBounceForce = impactSpeed * bounceForce * bounceAcceleration;
                float dynamicBounceForce = Mathf.Clamp(rawBounceForce, minBounceVelocity, maxBounceVelocity);

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, dynamicBounceForce);

                // Debug log to verify the contact normal and bounce force
                // Debug.Log($"Contact Normal: {contactNormal}, Final Bounce Force: {dynamicBounceForce}");
            }
        }
    }
}
