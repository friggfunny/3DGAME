using UnityEngine;

public class CombinedPlayerController : MonoBehaviour
{
    // YOUR MOUSE LOOK VARIABLES (sensitivity, camera transform, etc.)
    public Camera playerCamera;
    public float mouseSensitivity = 100f;
    float xRotation = 0f;


    // BUNNY HOP VARIABLES
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float airMultiplier = 0.4f;
    public float groundDrag = 6f;

    [Header("Jumping")]
    public float jumpForce = 10f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    private bool isGrounded;

    // Private variables
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked; // Typical for FPS controllers
    }

    void Update()
    {
        // --- THIS IS YOUR OLD MOUSE LOOK LOGIC ---
        MouseLook();
        // ------------------------------------------

        // --- THIS IS THE NEW B-HOP LOGIC ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        GetInput();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        rb.linearDamping = isGrounded ? groundDrag : 0;
        // ------------------------------------
    }

    void FixedUpdate()
    {
        // All movement physics are handled here
        MovePlayer();
    }

    // == YOUR MOUSE LOOK METHOD(S) ==
    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // == BUNNY HOP METHODS ==
    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}