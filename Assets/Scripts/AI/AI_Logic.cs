using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Timeline;

public class AI_Logic : MonoBehaviour
{
    private AI_Movement_ModernClutch movementLogic;//this creates a variable that will store a reference to another script
    private AI_TurretController turretLogic;
    public Transform PointA;//to know the position, rotation and scale of PointA
    public float aimDistance = 7f;

    void Start()
    {
        movementLogic = GetComponent<AI_Movement_ModernClutch>();//finds the movement script 
        turretLogic = GetComponentInChildren<AI_TurretController>();
    }

    void Update()
    {
        if (movementLogic == null || PointA == null)//if something not assignded so nothing heppens
            return;//stop executing this function

        Vector2 currentPosition = transform.position;//this objects current position
        Vector2 targetPosition = PointA.position;//this is PointsA current position
        Vector2 direction = targetPosition - currentPosition;//subracts targets postion with current and tells how much (x, y) to go to get up to the target
        float distance = direction.magnitude;//gets the length/distance 

        if (distance < 0.1f)//means that if distance between target and the object less then 0.1 then its arrived to the destination
        {
            movementLogic.moveInput = 0f;//
            movementLogic.turnInput = 0f;//basiclly tells to stop
            return;//no more calculations needed
        }

        direction /= distance;//this called "NORMALIZED VECTOR" It keeps only the direction not the distance
        Vector2 forward = (Vector2)transform.up;//FInds where the front of the object is pointing 

        float angleToTarget = Vector2.SignedAngle(forward, direction);//It basiccly askes how many degrees do I rotate from my current forward direction until I face the target?
        float turnInput = Mathf.Clamp(angleToTarget / 90f, -1f, 1f);// Convertes angle into steering
        float moveInput = (distance > 0.5f && Mathf.Abs(angleToTarget) < 25f) ? 1f : 0f;//Decides Whether to drive

        movementLogic.moveInput = moveInput;//those are basically sends the commands to the movement script 
        movementLogic.turnInput = turnInput;//for it to move
        //
        
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
