using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController_ModernClutch : MonoBehaviour, Tank
{
    float currentSpeed;
    float currentTurn;
    private bool movementEnabled = true;
    public LevelIntro Intro;

    public float CurrentSpeed { get; private set; }

    public void DisableMovement() => movementEnabled = false;
    public void EnableMovement() => movementEnabled = true;


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
    public TankModule leftTrack;
    public TankModule engine;
    public TankModule rightTrack;
    public TankModule driver;

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
        if (Intro != null && !LevelIntro.gameplayStarted)
        {
            movementEnabled = false;
            return;
        }
        movementEnabled = true;
    }

    void FixedUpdate()
    {
        
        Vector2 centerOffset = hullCenter != null ? (Vector2)(hullCenter.position - transform.position) : Vector2.zero;
        if (!movementEnabled || engine.IsDestroyed || leftTrack.IsDestroyed || rightTrack.IsDestroyed || driver.IsDestroyed)
        {
            currentSpeed = 0f;
            currentTurn = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }
        Vector2 forwardBeforeMovement = transform.up;
        float actualForwardSpeed = Vector2.Dot(rb.linearVelocity, forwardBeforeMovement);
        if (Mathf.Abs(actualForwardSpeed) < Mathf.Abs(currentSpeed) - 0.5f)
        {
            currentSpeed = actualForwardSpeed;
        }
        Vector2 forward = transform.up;

        bool canMove = !leftTrack.IsDestroyed && !rightTrack.IsDestroyed;

        if (!canMove)
        {
            currentSpeed = 0f;
            currentTurn = 0f;
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