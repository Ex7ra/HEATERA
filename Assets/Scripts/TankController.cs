using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour
{
    [Header("Speed (m/s)")]
    public float maxForwardSpeed = 13.9f;
    public float maxReverseSpeed = 2.2f;

    [Header("Acceleration")]
    public float acceleration = 3f;
    public float deceleration = 5f;

    [Header("Turning")]
    public float turnBoost = 8f;
    public float turnStrength = 6f;

    [Header("Modules")]
    public float engineHealth = 1f;
    public float leftTrackHealth = 1f;
    public float rightTrackHealth = 1f;

    [Header("Forward Axis")]
    public bool spriteFacesRight = false;

    Rigidbody2D rb;

    float leftTrackSpeed;
    float rightTrackSpeed;

    float moveInput;
    float turnInput;

    public float CurrentSpeed => Mathf.Abs((leftTrackSpeed + rightTrackSpeed) * 0.5f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 1f;
        rb.angularDamping = 3f;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (engineHealth <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 forward = spriteFacesRight ? (Vector2)transform.right : (Vector2)transform.up;

        float targetLeft = 0f;
        float targetRight = 0f;

        // Forward / Reverse base speed
        if (moveInput > 0)
        {
            targetLeft = maxForwardSpeed;
            targetRight = maxForwardSpeed;
        }
        else if (moveInput < 0)
        {
            targetLeft = -maxReverseSpeed;
            targetRight = -maxReverseSpeed;
        }

        // Turning adds speed difference
        if (turnInput < 0) // A
            targetRight += turnBoost;

        if (turnInput > 0) // D
            targetLeft += turnBoost;

        // Clamp track speeds
        targetLeft = Mathf.Clamp(targetLeft, -maxReverseSpeed, maxForwardSpeed);
        targetRight = Mathf.Clamp(targetRight, -maxReverseSpeed, maxForwardSpeed);

        // Acceleration / deceleration
        float leftRate = Mathf.Abs(targetLeft) > Mathf.Abs(leftTrackSpeed) ? acceleration : deceleration;
        float rightRate = Mathf.Abs(targetRight) > Mathf.Abs(rightTrackSpeed) ? acceleration : deceleration;

        leftTrackSpeed = Mathf.MoveTowards(leftTrackSpeed, targetLeft, leftRate * Time.fixedDeltaTime);
        rightTrackSpeed = Mathf.MoveTowards(rightTrackSpeed, targetRight, rightRate * Time.fixedDeltaTime);

        // Forward motion
        float forwardSpeed = (leftTrackSpeed + rightTrackSpeed) * 0.5f;
        rb.linearVelocity = forward * forwardSpeed;

        // Rotation
        float rotation = (rightTrackSpeed - leftTrackSpeed) * turnStrength;
        rb.MoveRotation(rb.rotation + rotation * Time.fixedDeltaTime);
    }
}