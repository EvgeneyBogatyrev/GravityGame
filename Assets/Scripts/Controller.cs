using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller : MonoBehaviour
{
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float inputSensitivity = 0.1f;
    private float currentSpeed = 0.0f;

    [SerializeField] private float groundedCounterMax = 60f;
    private float groundedCounter = 0f;

    private Vector2 savedAxis = Vector2.zero;

    private Rigidbody rb;

    [SerializeField] private float walkspeed = 4.0f;
    [SerializeField] private float runspeed = 10.0f;
    [SerializeField] private float jumpspeed = 5.0f;
    [SerializeField] private float sensitivity = 2.0f; 

    [SerializeField] private BallController ball;
    [SerializeField] private GameObject camera;

    private GameObject bloodScreen;
    private GameObject redScreen;

    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float hp = 100f;
    [SerializeField] private float healSpeed = 0.1f;

    [SerializeField] private float standTimeMax = 3f;
    [SerializeField] private float standTime = 3f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   
        rb = gameObject.GetComponent<Rigidbody>();
        hp = maxHp;

        standTime = standTimeMax;

        bloodScreen = GameObject.Find("BloodScreen");
        redScreen = GameObject.Find("RedScreen");
    }

    
    void Update()
    {
        Look();
        // Control
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (ball.GetState() == BallController.BallStates.inHand)
            {
                ball.SetState(BallController.BallStates.free);
                Vector3 forward = camera.transform.TransformDirection(Vector3.forward);
                ball.Throw(camera.transform.position + forward, forward);
            }
            else if (ball.GetState() == BallController.BallStates.free)
            {
                ball.SetState(BallController.BallStates.blackHole);
            }
            else if (ball.GetState() == BallController.BallStates.blackHole)
            {
                ball.SetState(BallController.BallStates.free);
            }
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (ball.GetState() == BallController.BallStates.free)
            {
                ball.SetState(BallController.BallStates.inHand);
            }
        }

        // Heal
        if (hp < maxHp)
        {
            hp += healSpeed;

            bloodScreen.SetActive(true);
            redScreen.SetActive(true);

            RawImage bloodImage = bloodScreen.GetComponent<RawImage>();
            RawImage redImage = redScreen.GetComponent<RawImage>();

            Color bloodColor = bloodImage.color;
            bloodColor.a = (maxHp - hp) / maxHp * 1f;
            bloodImage.color = bloodColor;

            Color redColor = redImage.color;
            redColor.a = (maxHp - hp) / maxHp * 0.5f;
            redImage.color = redColor;

            

        }
        else
        {
            hp = maxHp;
            bloodScreen.SetActive(false);
            redScreen.SetActive(false);
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
        // Run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runspeed;
        }
        else
        {
            currentSpeed = walkspeed;
        }

        // Ground Check
        if (Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.0001f))
        {
            groundedCounter = groundedCounterMax;
        }
        else
        {
            groundedCounter -= Time.deltaTime;
        }


        // Jump
        if (Grounded())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpspeed, rb.linearVelocity.z);
                groundedCounter = 0f;
            }
        }
            
        bool noInput = false;
        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) < inputSensitivity 
            && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < inputSensitivity)
        {
            noInput = true;
        }

        if (noInput)
        {
            standTime -= Time.deltaTime;
        }
        else
        {
            standTime = standTimeMax;
        }

        // Lock player
        if (noInput
            && !Input.GetKey(KeyCode.Space)
            && Grounded()
            && ball.GetState() != BallController.BallStates.blackHole)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }


        Vector2 axis = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")) * currentSpeed;

        // Save momentum during jump
        if (Grounded())
        {
            savedAxis = axis;
        }
        else
        {
            axis += savedAxis * 0.3f;
        }

        Vector3 forward = new Vector3(-Camera.main.transform.right.z, 0.0f, Camera.main.transform.right.x);
        Vector3 wishDirection = (forward * axis.x + Camera.main.transform.right * axis.y + Vector3.up * rb.linearVelocity.y);
        rb.linearVelocity = wishDirection;   
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collect the sphere
        BallController otherObject = other.gameObject.GetComponent<BallController>();
        if (otherObject != null && otherObject.GetState() == BallController.BallStates.free)
        {
            ball = otherObject;
            ball.SetState(BallController.BallStates.inHand);
        }
    }

    public bool Grounded()
    {
        if (groundedCounter > 0)
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        hp = Mathf.Max(0, hp);
    }

    public float GetSTandTime()
    {
        return standTime;
    }
}
