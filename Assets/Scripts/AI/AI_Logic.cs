using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Timeline;

public class AI_Logic : MonoBehaviour
{
    private AI_Movement_ModernClutch modernMovementLogic;//this creates a variable that will store a reference to another script
    private AI_Movement_ClutchBraking oldMovementLogic;
    private AI_TurretController turretLogic;
    public Transform PointA;//to know the position, rotation and scale of PointA
    public float aimDistance = 7f;

    void Start()
    {
        modernMovementLogic = GetComponent<AI_Movement_ModernClutch>();//finds the movement script 
        oldMovementLogic = GetComponent<AI_Movement_ClutchBraking>();
        turretLogic = GetComponentInChildren<AI_TurretController>();
    }

    void Update()
    {
        
        if (modernMovementLogic == null && oldMovementLogic == null )//if something not assignded so nothing heppens
            return;//stop executing this function
        if(PointA == null)
            return;
        Vector2 currentPosition = transform.position;//this objects current position
        Vector2 targetPosition = PointA.position;//this is PointsA current position
        Vector2 direction = targetPosition - currentPosition;//subracts targets postion with current and tells how much (x, y) to go to get up to the target
        float distance = direction.magnitude;//gets the length/distance 

        if (distance < 0.1f)//means that if distance between target and the object less then 0.1 then its arrived to the destination
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
            
            Debug.Log("Arrived at the target!");
            return;//no more calculations needed
        }
        

        direction /= distance;//this called "NORMALIZED VECTOR" It keeps only the direction not the distance
        Vector2 forward;//Finds where the front of the object is pointing 
        if(oldMovementLogic != null){
            forward = oldMovementLogic.spriteFacesRight ? (Vector2)transform.right : (Vector2)transform.up;
            }
            else{ 
                forward = transform.up;
            }
        Debug.DrawRay(transform.position, transform.up * 3, Color.green);
        Debug.DrawRay(transform.position, direction * 3, Color.red);

        float angleToTarget = Vector2.SignedAngle(forward, direction);//It basiccly askes how many degrees do I rotate from my current forward direction until I face the target?
        float turnInput = Mathf.Clamp(-angleToTarget / 90f, -1f, 1f);// Convertes angle into steering
        float moveInput = (distance > 0.5f && Mathf.Abs(angleToTarget) < 25f) ? 1f : 0f;//Decides Whether to drive
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
        

        if(distance < aimDistance){
        turretLogic.SetAimWorldPosition(PointA.position);
        Debug.Log("Aiming!");
        }
        else
        {
            Debug.Log("No target around!");
            return;
        }
        
    }
}
