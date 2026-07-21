using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AI_Movement_ClutchBraking : MonoBehaviour, ITankMovement
{
    private bool movementEnabled = true;
    public float CurrentSpeed { get; private set; }

    public void DisableMovement()
    {
        movementEnabled = false;
    }
    public void EnableMovement()
    {
        movementEnabled = true;
    }
    public void DestroyLeftTrack()
    {
        leftTrackHealth = 0f;
    }
    public void DestroyRightTrack()
    {
        rightTrackHealth = 0f;
    }
    public void EngineDestroyed()
    {
        engineHealth = 0f;
    }

    [Header("Speeds (km/h)")]
    public float forwardSpeedKmh = 50f;
    public float reverseSpeedKmh = 8f;

    [Header("Acceleration / Deceleration (m/s²)")]
     public float acceleration = 3f;
    public float deceleration = 5f;

    [Header("Turning")]
    public float pivotTurnDegPerSec = 20f;

    [Header("Turn Acceleration / Deceleration (m/s²)")]
     public float turnAcceleration = 2f;
    public float turnDeceleration = 6f;
    

    [Header("Tracks")]
    public Transform leftTrack;
    public Transform rightTrack;

    [Header("Modules Health")]
    public float engineHealth     = 1f;
    public float leftTrackHealth  = 1f;
    public float rightTrackHealth = 1f;

    [Header("Forward Axis")]
    public bool spriteFacesRight = false;

    Rigidbody2D rb;

    public float moveInput;
    public float turnInput;
    float previousTurnInput = 0f;
    float currentLeftSpeed  = 0f;
    float currentRightSpeed = 0f;


    float ForwardSpeedMs => forwardSpeedKmh / 3.6f;
    float ReverseSpeedMs => reverseSpeedKmh / 3.6f;
    float PivotTrackSpeed(float trackWidth)
    => pivotTurnDegPerSec * Mathf.Deg2Rad * (trackWidth * 0.5f);
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale   = 0f;
        rb.linearDamping  = 1f;
        rb.angularDamping = 3f;
    }

    void Update()
    {
        //moveInput = Input.GetAxisRaw("Vertical");
        //turnInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (!movementEnabled)
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }
        if (engineHealth <= 0f)
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }
        if (leftTrackHealth <= 0f || rightTrackHealth <= 0f)
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        Vector2 forward = spriteFacesRight
            ? (Vector2)transform.right
            : (Vector2)transform.up;

        float trackWidth = (leftTrack != null && rightTrack != null)
            ? Vector2.Distance(leftTrack.position, rightTrack.position)
            : 2f;

        float pivotSpeed = PivotTrackSpeed(trackWidth);

        float targetLeft  = 0f;
        float targetRight = 0f;

        if (moveInput == 0f && turnInput == 0f)
        {
            targetLeft  = 0f;
            targetRight = 0f;
        }
        else if (moveInput == 0f)
        {
            if (turnInput < 0f) 
            {
                targetLeft  = 0f;
                targetRight = pivotSpeed;
            }
            else               
            {
                targetLeft  = pivotSpeed;
                targetRight = 0f;
            }
        }
        else
        {
            float baseSpeed = moveInput > 0f ? ForwardSpeedMs : -ReverseSpeedMs;

            if (turnInput < 0f)      
            {
                targetLeft  = baseSpeed * (1f + turnInput); 
                targetRight = baseSpeed;
            }
            else if (turnInput > 0f)  
            {
                targetLeft  = baseSpeed;
                targetRight = baseSpeed * (1f - turnInput);
            }
            else                   
            {
                targetLeft  = baseSpeed;
                targetRight = baseSpeed;
            }
        }

        float sharedBase = moveInput != 0f
            ? (moveInput > 0f ? ForwardSpeedMs : -ReverseSpeedMs)
            : 0f;

        bool leftIsSteering  = !Mathf.Approximately(targetLeft,  sharedBase);
        bool rightIsSteering = !Mathf.Approximately(targetRight, sharedBase);

        if (previousTurnInput != 0f && turnInput == 0f && moveInput != 0f)
        {
           float averageSpeed = (currentLeftSpeed + currentRightSpeed) * 0.5f;

            currentLeftSpeed = averageSpeed;
            currentRightSpeed = averageSpeed;
        }
        else{
        currentLeftSpeed = StepSpeed(
        currentLeftSpeed, targetLeft,
        leftIsSteering  ? turnAcceleration : acceleration,
        leftIsSteering  ? turnDeceleration : deceleration);
        
        currentRightSpeed = StepSpeed(
        currentRightSpeed, targetRight,
        rightIsSteering ? turnAcceleration : acceleration,
        rightIsSteering ? turnDeceleration : deceleration);
        }

        float combinedSpeed = (currentLeftSpeed + currentRightSpeed) * 0.5f;
        combinedSpeed = Mathf.Clamp(combinedSpeed, -ReverseSpeedMs, ForwardSpeedMs);
        rb.linearVelocity = forward * combinedSpeed;

        float maxAngularVelocityRad = pivotTurnDegPerSec * Mathf.Deg2Rad;
        float angularVelocityRad = (currentRightSpeed - currentLeftSpeed) / trackWidth;
        angularVelocityRad = Mathf.Clamp(angularVelocityRad, -maxAngularVelocityRad, maxAngularVelocityRad);
        rb.angularVelocity = angularVelocityRad * Mathf.Rad2Deg;

        CurrentSpeed = Mathf.Abs(combinedSpeed);

        previousTurnInput = turnInput;
    }

    float StepSpeed(float current, float target, float accel, float decel)
    {
        float diff   = target - current;
        bool slowing = (Mathf.Abs(target) < Mathf.Abs(current))
                    && (target == 0f || Mathf.Sign(target) == Mathf.Sign(current));
        float rate   = slowing ? decel : accel;
        return current + Mathf.Sign(diff) * Mathf.Min(Mathf.Abs(diff), rate * Time.fixedDeltaTime);
    }

    public float SpeedKmh => CurrentSpeed * 3.6f;
}