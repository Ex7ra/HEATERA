using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour
{
    [Header("Speed (m/s)")]
    public float maxForwardSpeed = 13.9f; //in km/h = 50
    public float maxReverseSpeed = 2.2f;  //somewhere 8 km/h

    [Header("Acceleration (m/s^2)")]
    public float acceleration = 2.0f;     
    public float deceleration = 3.0f;     
    public float brakeDeceleration = 6.0f;

    [Header("Module Health (0..1)")]
    public float engineHealth = 1f;
    public float leftTrackHealth = 1f;
    public float rightTrackHealth = 1f;

    [Header("Turning (deg/sec)")]
    public float turnRateStopped = 18f;   
    public float turnRateMoving = 10f;    

    [Header("Forward Axis")]
    [Tooltip("Most top-down tanks face UP. If your sprite faces RIGHT, tick this.")]
    public bool spriteFacesRight = false;

    Rigidbody2D rb;
    float currentSpeed; 
    float moveInput;
    float turnInput;

    // UI 
    public float CurrentSpeed => Mathf.Abs(currentSpeed);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 1.0f;
        rb.angularDamping = 3.0f;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");   // W/S
        turnInput = Input.GetAxisRaw("Horizontal"); // A/D
    }

    void FixedUpdate()
    {
        // --- NEW: Disable movement if engine or at least one track is destroyed ---
        if (engineHealth <= 0f || leftTrackHealth <= 0f || rightTrackHealth <= 0f)
        {
            currentSpeed = 0f;
            rb.linearVelocity = Vector2.zero;
            return; // Skip all movement/rotation
        }

        Vector2 forward = spriteFacesRight ? (Vector2)transform.right : (Vector2)transform.up;

        float targetSpeed = 0f;
        if (moveInput > 0f) targetSpeed = maxForwardSpeed;
        else if (moveInput < 0f) targetSpeed = -maxReverseSpeed;

        bool reversingDirection = Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) && Mathf.Abs(currentSpeed) > 0.2f && Mathf.Abs(moveInput) > 0.1f;

        float rate;
        if (Mathf.Abs(moveInput) > 0.01f)
            rate = reversingDirection ? brakeDeceleration : acceleration;
        else
            rate = deceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);

        rb.linearVelocity = forward * currentSpeed;

        float speedRatio = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxForwardSpeed);
        float turnRate = Mathf.Lerp(turnRateStopped, turnRateMoving, speedRatio);

        float rotationDelta = -turnInput * turnRate * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotationDelta);
    }
}
