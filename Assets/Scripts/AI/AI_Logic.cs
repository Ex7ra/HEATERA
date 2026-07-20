using UnityEngine;
using Pathfinding;
using UnityEditor;
//Gives "Seeker" and "Path" these calculate a safe route around obstacles

public class AI_Logic : MonoBehaviour
{
    private LayerMask TankHull;
    private AI_Movement_ModernClutch modernMovementLogic;//this creates a variable that will store a reference to another script
    private AI_Movement_ClutchBraking oldMovementLogic;
    private AI_TurretController turretLogic;
    public Transform Point;//to know the position, rotation and scale of PointA
    public float aimDistance = 7f;//distance on which turret will start aiming 
    public float stoppingDistance = 1.5f;//how close/far will Bot stop near the something(Target, Point)
    private bool driveForward = true;// "True" then bot drives forward if "False" uses revurse
    public float obstacleDistance = 8f;//how far AI/bot should react to an obstacle
    public Transform turret;
    public Transform firePoint;
    public float targetUpdateRate = 1f;
    private float targetTimer;
    public Transform Enemy;
    //for pathfiner 
    private Seeker seeker;//Askes to find a path
    private Path path;//Gives path with points
    //
    private int currentWaypoint = 0;//which points of the path are we following now?

    public float waypointDistance = 3f;//Tells the AI/bot "When I'm this close to the current waypoint, start heading toward the next one."
    public float pathUpdateRate = 2f;//"How often should I calculate a brand new path?"
    private TankModule currentAimTarget;
    void Start()
    {
        modernMovementLogic = GetComponent<AI_Movement_ModernClutch>();//finds the movement script 
        oldMovementLogic = GetComponent<AI_Movement_ClutchBraking>();
        turretLogic = GetComponentInChildren<AI_TurretController>();//InChildren because its in children gameObject
        seeker = GetComponent<Seeker>();

        InvokeRepeating(nameof(UpdatePath),0f,pathUpdateRate);//updates the path my every number which is set in "pathUpdateRate" because targets move
        if(gameObject.tag != "Blue")
        {
            gameObject.tag = "Red";
            Debug.Log("I am on " + tag + " team");
        }
        else
        {
            Debug.Log("I am on " + tag + " team");
        }
        
    }
    void FindEnemy()
    {
        GameObject[] enemies;

        if(gameObject.tag == "Red")
        {
            enemies = GameObject.FindGameObjectsWithTag("Blue");
        }
        else
        {
            enemies = GameObject.FindGameObjectsWithTag("Red");
        }

        float closestDistance = Mathf.Infinity;

        foreach(GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                Enemy = enemy.transform;
                Point = Enemy;
                ChooseAimPoint();
            }
        }
    }
    void UpdatePath()//This creates a route
    {
        if(Point == null)//checks if Points exists if not then stop "return"
            return;

        if(seeker.IsDone())//checks if seeker is free (a path cannot be calculated while another running)
        {
            seeker.StartPath(transform.position, Point.position, OnPathComplete);//Creates a path from Start "transf.pos" to End "Pointa.pos" when finished call "OnPathComplete"
        }
    }
    void OnPathComplete(Path p)//Receving the path
    {
        if(!p.error)//MEANS: if there was no error
        {
            path = p;//Store it
            currentWaypoint = 0;// start from the begining
        }
    }
    
    
    void Update()
    {
        targetTimer -= Time.deltaTime;
        if(targetTimer <= 0)
        {
            FindEnemy();
            targetTimer = targetUpdateRate;
        }
        if (modernMovementLogic == null && oldMovementLogic == null )//if something not assignded so nothing heppens
        return;//stop executing this function
        if(Point == null)
        return;
        if(path == null)
        return;

        Vector2 currentPosition = transform.position;//this objects current position
        Vector2 targetPosition = path.vectorPath[currentWaypoint];//Gets current waypoint
        Vector2 direction = targetPosition - currentPosition;//subracts targets postion with current and tells how much (x, y) to go to get up to the target
        float distance = direction.magnitude;//gets the length/distance 
        
       

        float targetDistance = Vector2.Distance(transform.position,Point.position);//I want the distance between these two positions.

        AimingTheTurret(targetDistance);
        ShootTheTarget(targetDistance);
        if(distance < waypointDistance)//Moving to next waypoint, if yes "currentWaypoint++;" go to next point
        {
            currentWaypoint++;

            if(currentWaypoint >= path.vectorPath.Count)//Checks if there are any more waypoints left.
            {
                Stop();
                return;
            }

            targetPosition = path.vectorPath[currentWaypoint];//gets next point from the path

            direction = targetPosition - currentPosition;//Calculates the direction from the tank to the new waypoint.
            distance = direction.magnitude;//Calculates how far away the new waypoint is.
        }

        if(targetDistance < stoppingDistance)//Stops at the destination
        {
            Debug.Log("Arrived at target");
            Stop();
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count)//checks whether the AI has reached the end of the path.
        {
            Debug.Log("Pathway complited!");
            Stop();
            return;
        }

        
        direction /= distance;//this called "NORMALIZED VECTOR" It keeps only the direction not the distance
        Vector2 forward;//Finds where the front of the object is pointing 
        if(oldMovementLogic != null)
        {
            forward = oldMovementLogic.spriteFacesRight ? (Vector2)transform.right : (Vector2)transform.up;//(short if/else) that decides which direction is the front of the tank.
        }
        else
        { 
            forward = transform.up;
        }
        Vector2 backward = -forward;//backward is negative forward
        Debug.DrawRay(transform.position, -transform.up * 3, Color.blue);
        Debug.DrawRay(transform.position, direction * 3, Color.red);
        
        float angleToTargetForward = Vector2.SignedAngle(forward, direction);//It basiccly askes how many degrees do I rotate from my current forward direction until I face the target?
        float angleToTargetBackwards = Vector2.SignedAngle(backward, direction);//same as last but bakward
        float forwardAngle = Mathf.Abs(angleToTargetForward);//They are used because the AI only cares about how big the turn is, not whether it is left or right.
        float backwardAngle = Mathf.Abs(angleToTargetBackwards);
        //Debug.Log($"Forward: {forwardAngle}  Backward: {backwardAngle}  DriveForward: {driveForward}");
     
        if(driveForward)//chooses forawrd or reverse 
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

        float chosenAngle = driveForward ? angleToTargetForward : angleToTargetBackwards;//"Which direction should I use for steering?"
        float turnInput;//"How much should I turn the tank?"

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
        }
        else
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
            float speedAmount = Mathf.Clamp01(0.5f - angleError / 45f);//This slows the tank when turning

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

    
    }
    void Stop()//function which make tank to stop
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

    void AimingTheTurret(float distance)
    {
        Debug.DrawRay(firePoint.position, -turret.right  * distance, Color.yellow);
        if(distance < aimDistance)//Turret aiming
        {
            if(currentAimTarget != null)
            {
                turretLogic.SetAimWorldPosition(currentAimTarget.transform.position);
            }
            else
            {
                turretLogic.SetAimWorldPosition(Point.position);
            }
            Debug.Log("Aiming!");
        }
        else
        {   
            Debug.Log("No target around!");
        }
    }
    
    void ShootTheTarget(float distance)
    {
        if(Enemy == null)
            return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, -turret.right, distance);

        if (hits == null || hits.Length == 0)
            return;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            Transform tank = hit.collider.transform.root;

            if(tank == transform)
                continue;//for ignoring it self

            if(!hit.collider.CompareTag(Enemy.tag))
            {
                continue;
            }
            Debug.Log("Enemy tank on sight: " + tank.name);
            turretLogic.TryShoot();
            return;
            
            
        }
        
    }
    void ChooseAimPoint()
    {
        if(Enemy == null)
            return;

        TankModule[] modules = Enemy.GetComponentsInChildren<TankModule>();

        if(modules.Length == 0)
            return;

        currentAimTarget = modules[Random.Range(0, modules.Length)];
        
    }
    
}   
