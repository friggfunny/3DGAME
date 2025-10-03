using UnityEngine;

/// <summary>
/// A character controller script that mimics movement physics from the Source engine,
/// including bunny hopping and air strafing.
/// This script requires a CharacterController component on the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SourceMovementController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float lookSpeedX = 2f;
    public float lookSpeedY = 2f;

    [Header("Movement Speeds")]
    public float moveSpeed = 7.0f;          // Ground speed
    public float maxSpeed = 15.0f;          // Maximum overall speed
    public float groundAcceleration = 10.0f; // Ground acceleration
    public float airAcceleration = 150.0f;   // Air acceleration
    public float groundFriction = 6.0f;     // How quickly you slow down on the ground

    [Header("Jumping")]
    public float jumpForce = 8.0f;          // The force of the jump
    public float gravity = 20.0f;           // Gravity force

    private CharacterController characterController;
    private Vector3 velocity = Vector3.zero;

    private float rotationX = 0f;
    private float rotationY = 0f;

    // Player input
    private float moveInputX;
    private float moveInputZ;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Add a guard clause to prevent errors if the controller is inactive.
        if (characterController == null || !characterController.enabled)
        {
            return;
        }

        HandleMouseLook();
        GetPlayerInput();

        // Queue a jump if the button is pressed
        if (Input.GetButton("Jump"))
        {
            QueueJump();
        }

        if (characterController.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;

        // Move the character controller
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void GetPlayerInput()
    {
        // Get WASD input
        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputZ = Input.GetAxisRaw("Vertical");
    }

    private void QueueJump()
    {
        // Allow jumping if grounded or if you have a little bit of upward velocity.
        // This makes bunny hopping feel more responsive.
        if (characterController.isGrounded)
        {
            velocity.y = jumpForce;
        }
    }

    /// <summary>
    /// Handles movement logic when the player is on the ground.
    /// </summary>
    private void GroundMove()
    {
        // Apply friction
        ApplyFriction(1.0f);

        // Get the direction the player wants to move in, based on input
        Vector3 wishDirection = new Vector3(moveInputX, 0, moveInputZ);
        wishDirection = transform.TransformDirection(wishDirection);
        wishDirection.Normalize();

        float wishSpeed = wishDirection.magnitude;
        wishSpeed *= moveSpeed;

        // Accelerate the player
        Accelerate(wishDirection, wishSpeed, groundAcceleration);

        // Reset the y velocity if grounded
        velocity.y = -gravity * Time.deltaTime;

        // Handle jumping
        if (Input.GetButton("Jump"))
        {
            QueueJump();
        }
    }

    /// <summary>
    /// Handles movement logic when the player is in the air.
    /// </summary>
    private void AirMove()
    {
        Vector3 wishDirection = new Vector3(moveInputX, 0, moveInputZ);
        wishDirection = transform.TransformDirection(wishDirection);

        float wishSpeed = wishDirection.magnitude;
        wishSpeed *= moveSpeed;

        // Air strafe logic
        Accelerate(wishDirection, wishSpeed, airAcceleration);

        // Apply air friction if the player is not trying to move
        if (Mathf.Abs(moveInputX) < 0.01f && Mathf.Abs(moveInputZ) < 0.01f)
        {
            ApplyFriction(0.5f); // Less friction in the air
        }
    }

    /// <summary>
    /// Applies friction to the player's velocity.
    /// </summary>
    private void ApplyFriction(float multiplier)
    {
        Vector3 vec = velocity;
        float speed = vec.magnitude;

        // Don't apply friction if the player isn't moving
        if (speed < 0.01f)
            return;

        vec.y = 0; // Don't apply friction to vertical movement

        float drop = speed * groundFriction * multiplier * Time.deltaTime;
        float newSpeed = speed - drop;

        if (newSpeed < 0)
            newSpeed = 0;

        if (speed > 0)
            newSpeed /= speed;

        velocity.x *= newSpeed;
        velocity.z *= newSpeed;
    }

    /// <summary>
    /// Accelerates the player's velocity in a given direction.
    /// This is the core of air-strafing.
    /// </summary>
    private void Accelerate(Vector3 wishDirection, float wishSpeed, float acceleration)
    {
        // The "magic" of air strafing is in this dot product.
        // It projects the wish direction onto the current velocity, which allows you to gain speed by looking away from your direction of travel.
        float currentSpeed = Vector3.Dot(velocity, wishDirection);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0)
            return;

        float accelSpeed = acceleration * wishSpeed * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity.x += accelSpeed * wishDirection.x;
        velocity.z += accelSpeed * wishDirection.z;

        // Clamp to max speed
        if (new Vector3(velocity.x, 0, velocity.z).magnitude > maxSpeed)
        {
            Vector3 clampedVel = new Vector3(velocity.x, 0, velocity.z).normalized * maxSpeed;
            velocity.x = clampedVel.x;
            velocity.z = clampedVel.z;
        }
    }
}

