using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]//prevents the script from working without a Rigidbody2D
// Automatically adds a Rigidbody2D when this script is attached
public class TankController_ClutchBraking : MonoBehaviour, Tank
{
    private bool movementEnabled = true;//by default TRUE allows movement, if FALSE the tank will not move, used for when the tank is destroyed or immobilized
    public float CurrentSpeed { get; private set; }// "get" is so any script is allowed to read it, "private set" means only this script could change it 
    public bool isAccelerating {get; private set; }
    private float previousSpeed;
    public void DisableMovement()//function/method, when its called tank stops moving
    {
        movementEnabled = false;
    }
    public void EnableMovement()//funcation/method, turnes movement back by setting "movementEnbaled = true" 
    {
        movementEnabled = true;
    }
    
    //These are tunable settings exposed to the Unity Inspector that control movement, turning, and health behaviour of the tank
    
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

    [Header("Health")]
    public TankModule leftTrackModule;
    public TankModule engine;
    public TankModule rightTrackModule;
    public TankModule driver;

    [Header("Forward Axis")]
    public bool spriteFacesRight = false;
    //
    Rigidbody2D rb;//Stores a reference to the Rigidbody2D component so the script can control the tank’s physics (movement and rotation)

    float moveInput;
    //These variables store the player’s keyboard input values, which are updated every frame and used to control the tank’s movement
    float turnInput;
    float previousTurnInput = 0f;
    float currentLeftSpeed  = 0f;
    //These store the actual current speeds of each track and are used to smoothly transition toward target movement values instead of changing instantly
    float currentRightSpeed = 0f;


    float ForwardSpeedMs => forwardSpeedKmh / 3.6f;
    //These are read-only properties that convert speed from km/h to m/s for use in Unity physics
    float ReverseSpeedMs => reverseSpeedKmh / 3.6f;
    float PivotTrackSpeed(float trackWidth)//Calculates the required speed of a tank track during pivot turning based on turn speed and track width
    => pivotTurnDegPerSec * Mathf.Deg2Rad * (trackWidth * 0.5f);//
    void Awake()//Awake works earliar then void Start for initial setup of references before game actually starts running logic
    {
        rb = GetComponent<Rigidbody2D>();//means: find the Rigidbody2D attached to this GameObject and store it in rb
        rb.gravityScale   = 0f;// dont lets gravity pull the object down 
        rb.linearDamping  = 1f;//slows down movement over time (like friction in air/ground)
        rb.angularDamping = 3f;//Slow down rotation over time
    }//Initializes the Rigidbody2D reference and configures physics settings (no gravity, movement and rotation friction) before the game starts

    void Update()//runs every frame for input and logic   
    {
        moveInput = Input.GetAxisRaw("Vertical");//"Vertical" and "Horizontal" are predefined Unity input axes that are already mapped to WASD and arrow keys in the Input Manager
        turnInput = Input.GetAxisRaw("Horizontal");//Reads realtime keyboard input for forward/backward and turning control of the tank
    }

    void FixedUpdate()//runs at a fixed rate for physics calculations like Rigidbody movement
    {
        
        if (!movementEnabled || engine == null || engine.IsDestroyed ||leftTrackModule == null || leftTrackModule.IsDestroyed || rightTrackModule == null || rightTrackModule.IsDestroyed || driver == null || driver.IsDestroyed)
        {
            currentLeftSpeed = 0f;
            currentRightSpeed = 0f;
            CurrentSpeed = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            previousTurnInput = 0f;

            return;
        }
        
        Vector2 forward = spriteFacesRight//Chooses the tanks forward movement direction based on whether the sprite is oriented right or up
            ? (Vector2)transform.right //NOTE: "?" means if
            : (Vector2)transform.up;// ":" means else

        float trackWidth = (leftTrack != null && rightTrack != null)//Calculates the distance between left and right tracks if both exist; otherwise uses a default width of 2 units
            ? Vector2.Distance(leftTrack.position, rightTrack.position)
            : 2f;

        float pivotSpeed = PivotTrackSpeed(trackWidth);//Calculates the required track speed for pivot turning based on the distance between the track

        float targetLeft  = 0f;
        float targetRight = 0f;

        if (moveInput == 0f && turnInput == 0f)//This code decides the speed of each track based on movement and turning input, allowing the tank to move straight, turn, or pivot like a real tank
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
        }//
        
        float sharedBase = moveInput != 0f//Calculates the base movement speed depending on whether the player is moving forward, backward, or not moving at all
            ? (moveInput > 0f ? ForwardSpeedMs : -ReverseSpeedMs)
            : 0f;
       
        //Checks whether each track’s target speed differs from normal movement speed, meaning the tank is currently steering
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
        //Smoothly adjusts the of track’s current speed toward its target speed using different acceleration values depending on whether the tank is turning or moving straight
        currentRightSpeed = StepSpeed(
        currentRightSpeed, targetRight,
        rightIsSteering ? turnAcceleration : acceleration,
        rightIsSteering ? turnDeceleration : deceleration);
        }
        float actualForwardSpeed = Vector2.Dot(rb.linearVelocity, forward);
        float intendedSpeed = (currentLeftSpeed + currentRightSpeed) * 0.5f;
        if (Mathf.Abs(actualForwardSpeed) < Mathf.Abs(intendedSpeed) - 0.5f)
        {
            currentLeftSpeed = actualForwardSpeed;
            currentRightSpeed = actualForwardSpeed;
        }
        //Combines left and right track speeds into a single clamped movement speed and applies it to move the tank forward in its facing direction
        float combinedSpeed = (currentLeftSpeed + currentRightSpeed) * 0.5f;
        combinedSpeed = Mathf.Clamp(combinedSpeed, -ReverseSpeedMs, ForwardSpeedMs);
        rb.linearVelocity = forward * combinedSpeed;
        //Calculates tank rotation speed based on the difference between left and right track speeds, then clamps and applies it as angular velocity
        float maxAngularVelocityRad = pivotTurnDegPerSec * Mathf.Deg2Rad;
        float angularVelocityRad = (currentRightSpeed - currentLeftSpeed) / trackWidth;
        angularVelocityRad = Mathf.Clamp(angularVelocityRad, -maxAngularVelocityRad, maxAngularVelocityRad);
        rb.angularVelocity = angularVelocityRad * Mathf.Rad2Deg;

        CurrentSpeed = rb.linearVelocity.magnitude;//Stores the absolute (non-negative) value of the tank’s speed, ignoring direction for UI or gameplay purposes
        previousTurnInput = turnInput;
        isAccelerating = CurrentSpeed > previousSpeed && CurrentSpeed < 2.78f;
        previousSpeed = CurrentSpeed;
    }
    //Gradually adjusts current speed toward target speed using acceleration or deceleration while preventing overshooting
    
    float StepSpeed(float current, float target, float accel, float decel)
    {
        float diff   = target - current;
        bool slowing = (Mathf.Abs(target) < Mathf.Abs(current))
                    && (target == 0f || Mathf.Sign(target) == Mathf.Sign(current));
        float rate   = slowing ? decel : accel;
        return current + Mathf.Sign(diff) * Mathf.Min(Mathf.Abs(diff), rate * Time.fixedDeltaTime);
    }
    
    public float CurrentTurn => rb != null ? rb.angularVelocity : 0f;
    public float SpeedKmh => CurrentSpeed * 3.6f;//Converts the tank’s current speed from meters per second into kilometers per hour for display or UI purposes
    
}