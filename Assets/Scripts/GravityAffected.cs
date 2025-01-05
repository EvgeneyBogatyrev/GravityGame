using UnityEngine;

public class GravityAffected : MonoBehaviour
{
    private BallController ball = null;
    private Rigidbody rb;

    private float upVectorScale = 3f;
    
    void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<BallController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ball == null)
        {
            ball = GameObject.Find("Ball").GetComponent<BallController>();
        }
        else if (ball.GetState() == BallController.BallStates.blackHole)
        {
            if (Grounded())
            {
                rb.AddForce(Vector3.up * upVectorScale * rb.mass);
            }

            Vector3 direction = (ball.gameObject.transform.position - rb.transform.position);
            float distance = direction.magnitude + 0.0001f;
            direction /= distance;
            distance = Mathf.Max(distance, 1f);
            rb.AddForce(direction * rb.mass * ball.GetGravityForce() / distance);
            
            /*
            Vector3 direction = ball.transform.position - rb.transform.position;
            float distance = direction.magnitude + 0.0001f;  // Prevent division by zero
            direction.Normalize();  // Ensure direction is always unit length
            distance = Mathf.Max(distance, 1f);  // Prevent excessive forces at close range

            // Apply gravitational pull
            float gravityForce = ball.GetGravityForce();
            rb.AddForce(direction * rb.mass * gravityForce / (distance));  // Inverse-square law for realism

            // Apply tangential force for spinning effect
            Vector3 tangent = Vector3.Cross(direction, Vector3.up);  // Tangent perpendicular to direction
            float spinForce = Mathf.Sqrt(gravityForce / distance);   // Spin faster closer to the black hole
            rb.AddForce(tangent * rb.mass * spinForce / 3f);
            */
        }
    }

    public bool Grounded()
    {
        return Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f);
    }
}
