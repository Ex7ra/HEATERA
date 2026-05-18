using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController_ModernClutch : MonoBehaviour, ITankMovement
{
    float currentSpeed;
    float currentTurn;
    private bool movementEnabled = true;

    public float CurrentSpeed { get; private set; }

    public void DisableMovement() => movementEnabled = false;
    public void EnableMovement() => movementEnabled = true;

    public void DestroyLeftTrack() => leftTrackHealth = 0f;
    public void DestroyRightTrack() => rightTrackHealth = 0f;
    public void EngineDestroyed() => engineHealth = 0f;

    [Header("Acceleration")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    [Header("Hull")]
    public Transform hullCenter;

    [Header("Speed")]
    public float forwardSpeedKmh = 50f;
    public float reverseSpeedKmh = 8f;

    [Header("Turn")]
    public float turnSpeed = 35f; // degrees per second
    [Header("Turning Acceleration")]
    public float turnAcceleration = 200f;
    public float turnDeceleration = 300f;

    [Header("Health")]
    public float engineHealth = 1f;
    public float leftTrackHealth = 1f;
    public float rightTrackHealth = 1f;

    Rigidbody2D rb;

    float moveInput;
    float turnInput;

    float ForwardSpeedMs => forwardSpeedKmh / 3.6f;
    float ReverseSpeedMs => reverseSpeedKmh / 3.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
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
            Vector2 centerOffset = hullCenter != null
        ? (Vector2)(hullCenter.position - transform.position)
        : Vector2.zero;
        if (!movementEnabled || engineHealth <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        Vector2 forward = transform.up;

        bool canMove = leftTrackHealth > 0f && rightTrackHealth > 0f;

        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        float targetSpeed =
        moveInput > 0 ? ForwardSpeedMs :
        moveInput < 0 ? -ReverseSpeedMs : 0f;

        
        float speedRate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)
            ? acceleration
            : deceleration;
         currentSpeed = Mathf.MoveTowards(currentSpeed,targetSpeed,speedRate * Time.fixedDeltaTime);
        
        float targetTurn = -turnInput * turnSpeed;
        float turnRate = Mathf.Abs(targetTurn) > Mathf.Abs(currentTurn)
            ? turnAcceleration
            : turnDeceleration;
        currentTurn = Mathf.MoveTowards(currentTurn, targetTurn, turnRate * Time.fixedDeltaTime);

        float rotationDelta = currentTurn * Time.fixedDeltaTime;

        transform.RotateAround(hullCenter.position, Vector3.forward, rotationDelta);
        rb.linearVelocity = forward * currentSpeed;

       
        CurrentSpeed = Mathf.Abs(currentSpeed);
    }

    public float SpeedKmh => CurrentSpeed * 3.6f;
}