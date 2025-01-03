using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallController : MonoBehaviour
{
    public enum BallStates
    {
        inHand,
        free,
        blackHole,
    }
    private BallStates state = BallStates.inHand; 
    [SerializeField] private GameObject inHandPosition;
    [SerializeField] private float ballSpeed = 5.0f;
    [SerializeField] private float gravityForce = 0.1f;

    private Rigidbody rb;
    private float baseMass = 1f;
    private float blackHoleMass = 100f;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (state)
        {
            case BallStates.inHand:
                rb.transform.position = inHandPosition.transform.position;
                rb.useGravity = false;
                rb.mass = baseMass;
                break;
            case BallStates.free:
                rb.mass = baseMass;
                rb.useGravity = true;
                break;
            case BallStates.blackHole:
                rb.useGravity = false;
                rb.mass = blackHoleMass;
                rb.linearVelocity = Vector3.zero;
                GameObject[] entities = GameObject.FindGameObjectsWithTag("GravityAffected");
                foreach (GameObject entity in entities)
                {
                    Vector3 direction = (transform.position - entity.transform.position);
                    direction /= (direction.magnitude + 0.0001f);
                    entity.GetComponent<Rigidbody>().AddForce(direction * gravityForce);
                }
                break;
        }
    }

    public BallStates GetState()
    {
        return state;
    }

    public void SetState(BallStates _state)
    {
        state = _state;
    }

    public void Throw(Vector3 angle)
    {
        rb.linearVelocity = angle * ballSpeed;
        rb.useGravity = true;
    }

    public bool Grounded()
    {
        return Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f);
    }
}
