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
    private BallStates state = BallStates.free; 
    [SerializeField] private GameObject inHandPosition;
    [SerializeField] private float ballSpeed = 5.0f;
    [SerializeField] private float gravityForce = 0.1f;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material blackholeMat;

    private Rigidbody rb;
    [SerializeField] private float baseMass = 5f;
    [SerializeField] private float blackHoleMass = 100f;

    [SerializeField] private float normalScale = 0.2f;
    [SerializeField] private float blackHoleScale = 0.7f;

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
                rb.GetComponent<MeshRenderer>().material = normalMat;
                rb.transform.localScale = new Vector3(normalScale, normalScale, normalScale);
                break;
            case BallStates.free:
                rb.mass = baseMass;
                rb.useGravity = true;
                rb.GetComponent<MeshRenderer>().material = normalMat;
                rb.transform.localScale = new Vector3(blackHoleScale, blackHoleScale, blackHoleScale);
                break;
            case BallStates.blackHole:
                rb.useGravity = false;
                rb.mass = blackHoleMass;
                rb.linearVelocity = Vector3.zero;
                GetComponent<MeshRenderer>().material = blackholeMat;
                rb.transform.localScale = new Vector3(blackHoleScale, blackHoleScale, blackHoleScale);
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

    public void Throw(Vector3 from, Vector3 angle)
    {
        rb.transform.position = from;
        rb.linearVelocity = angle * ballSpeed;
        rb.useGravity = true;
    }

    public bool Grounded()
    {
        return Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f);
    }

    public float GetGravityForce()
    {
        return gravityForce;
    }
}
