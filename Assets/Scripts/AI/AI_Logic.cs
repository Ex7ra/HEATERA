using UnityEngine;
using Pathfinding;

public class AI_Logic : MonoBehaviour
{
    private AI_Movement_ModernClutch modernMovementLogic;//this creates a variable that will store a reference to another script
    private AI_Movement_ClutchBraking oldMovementLogic;
    private AI_TurretController turretLogic;
    public Transform PointA;//to know the position, rotation and scale of PointA
    public float aimDistance = 7f;
    public float stoppingDistance = 1.5f;
    private bool driveForward = true;
    public float obstacleDistance = 8f;
    private bool avoidingObstacle = false;
    private float avoidTimer = 0f;
    //for pathfiner 
    private Seeker seeker;
    private Path path;

    private int currentWaypoint = 0;

    public float waypointDistance = 3f;
    public float pathUpdateRate = 2f;

    void Start()
    {
        modernMovementLogic = GetComponent<AI_Movement_ModernClutch>();//finds the movement script 
        oldMovementLogic = GetComponent<AI_Movement_ClutchBraking>();
        turretLogic = GetComponentInChildren<AI_TurretController>();
        seeker = GetComponent<Seeker>();

        InvokeRepeating(nameof(UpdatePath),0f,pathUpdateRate);
    }
    void UpdatePath()
    {
        if(PointA == null)
            return;

        if(seeker.IsDone())
        {
            seeker.StartPath(
                transform.position,
                PointA.position,
                OnPathComplete
            );
        }
    }
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    
    void Update()
    {
        
        if (modernMovementLogic == null && oldMovementLogic == null )//if something not assignded so nothing heppens
            return;//stop executing this function
        if(PointA == null)
            return;
            
        Vector2 currentPosition = transform.position;//this objects current position
        
        
        Vector2 targetPosition = path.vectorPath[currentWaypoint];//this is PointsA current position
        Vector2 direction = targetPosition - currentPosition;//subracts targets postion with current and tells how much (x, y) to go to get up to the target
        float distance = direction.magnitude;//gets the length/distance 

        if(path == null)
        return;

        float targetDistance = Vector2.Distance(transform.position,PointA.position);

        if(distance < waypointDistance)
        {
            currentWaypoint++;

            if(currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }

            targetPosition = path.vectorPath[currentWaypoint];

            direction = targetPosition - currentPosition;
            distance = direction.magnitude;
        }

        if(targetDistance < stoppingDistance)
        {
            Debug.Log("Arrived at target");
            Stop();
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count){
            Debug.Log("Pathway complited!");
            Stop();
            return;
        }

        
        direction /= distance;//this called "NORMALIZED VECTOR" It keeps only the direction not the distance
        Vector2 forward;//Finds where the front of the object is pointing 
        if(oldMovementLogic != null){
            forward = oldMovementLogic.spriteFacesRight ? (Vector2)transform.right : (Vector2)transform.up;
            }
            else{ 
                forward = transform.up;
            }
        Vector2 backward = -forward;
        Debug.DrawRay(transform.position, -transform.up * 3, Color.blue);
        Debug.DrawRay(transform.position, direction * 3, Color.red);
        
        float angleToTargetForward = Vector2.SignedAngle(forward, direction);//It basiccly askes how many degrees do I rotate from my current forward direction until I face the target?
        float angleToTargetBackwards = Vector2.SignedAngle(backward, direction);
        float forwardAngle = Mathf.Abs(angleToTargetForward);
        float backwardAngle = Mathf.Abs(angleToTargetBackwards);
        //Debug.Log($"Forward: {forwardAngle}  Backward: {backwardAngle}  DriveForward: {driveForward}");
     
        if(driveForward)
        {
            if(backwardAngle + 30f < forwardAngle)
            {
                driveForward = false;
            }
        }
        else
        {
            if(forwardAngle + 30f < backwardAngle)
            {
                driveForward = true;
            }
        }
        float chosenAngle = driveForward ? angleToTargetForward : angleToTargetBackwards;
        float turnInput;

        if (modernMovementLogic != null)// Convertes angle into steering
        {
            // for modern clutch steering
            turnInput = Mathf.Clamp(chosenAngle / 25f, -1f, 1f);
        }
        else
        {
            //for old movement steering
            turnInput = Mathf.Clamp(-chosenAngle / 25f, -1f, 1f);
        }
        if(modernMovementLogic == null)
        {
             if(!driveForward)
        {
            turnInput = -turnInput;
        }
        }else
        {
            if(!driveForward)
        {
            turnInput = turnInput;
        }
        }

        float angleError = Mathf.Abs(chosenAngle);//Decides Whether to drive
        float moveInput = 0f;

        if(distance > 0.5f)
        {
            float speedAmount = Mathf.Clamp01(1f - angleError / 45f);

            if(driveForward)
            {
                moveInput = speedAmount;
            }
            else
            {
                moveInput = -speedAmount;
            }
        }
        if(angleError > 60f)
        {
            moveInput *= 0.5f;
        }
        
        //
        if(oldMovementLogic != null)
        {
        oldMovementLogic.moveInput = moveInput;//those are basically sends the commands to the movement script 
        oldMovementLogic.turnInput = turnInput;//for it to move
        }
        if(modernMovementLogic != null)
        {
        modernMovementLogic.moveInput = moveInput;//those are basically sends the commands to the movement script 
        modernMovementLogic.turnInput = turnInput;//for it to move

        }
        //Debug.Log("Move: " + moveInput +" Turn: " + turnInput +" Angle: " + angleError);

        if(distance < aimDistance){
        turretLogic.SetAimWorldPosition(PointA.position);
        Debug.Log("Aiming!");
        }
        else
        {
            Debug.Log("No target around!");
        }
        
    }
    void Stop()
        {
            if(modernMovementLogic != null)
            {
                modernMovementLogic.moveInput = 0f;//
                modernMovementLogic.turnInput = 0f;//basiclly tells to stop
                
            }
            if(oldMovementLogic != null)
            {
                oldMovementLogic.moveInput = 0f;//
                oldMovementLogic.turnInput = 0f;//basiclly tells to stop
                
            }
        }
}   
