using UnityEngine;

public class Controller : MonoBehaviour
{
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Rigidbody rb;

    [SerializeField] private float walkspeed = 5.0f;
    [SerializeField] private float jumpspeed = 5.0f;
    [SerializeField] private float sensitivity = 2.0f;

    [SerializeField] private BallController ball;
    [SerializeField] private GameObject camera;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   
        rb = gameObject.GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpspeed, rb.linearVelocity.z);
        }
        Look();
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (ball.GetState() == BallController.BallStates.inHand)
            {
                ball.SetState(BallController.BallStates.free);
                Vector3 forward = camera.transform.TransformDirection(Vector3.forward);
                ball.Throw(forward);
            }
            else if (ball.GetState() == BallController.BallStates.free)
            {
                ball.SetState(BallController.BallStates.blackHole);
            }
            else
            {
                ball.SetState(BallController.BallStates.free);
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (ball.GetState() == BallController.BallStates.free)
            {
                ball.SetState(BallController.BallStates.inHand);
            }
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        yaw += Input.GetAxisRaw("Mouse X") * sensitivity;
        Camera.main.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }

    private void Movement()
    {
        Vector2 axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * walkspeed;
        Vector3 forward = new Vector3(-Camera.main.transform.right.z, 0.0f, Camera.main.transform.right.x);
        Vector3 wishDirection = (forward * axis.x + Camera.main.transform.right * axis.y + Vector3.up * rb.linearVelocity.y);
        rb.linearVelocity = wishDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        BallController otherObject = other.gameObject.GetComponent<BallController>();
        if (otherObject != null && otherObject.Grounded() && otherObject.GetState() == BallController.BallStates.free)
        {
            ball = otherObject;
            ball.SetState(BallController.BallStates.inHand);
        }
    }
}
