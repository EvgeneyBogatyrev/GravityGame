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
        }
    }

    public bool Grounded()
    {
        return Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f);
    }
}
