using UnityEngine;

public class AI_Logic : MonoBehaviour
{
    
    private AI_Movement_ClutchBraking movementLogic;
    public Transform PointA;
    
    void Start()
    {
        movementLogic = GetComponent<AI_Movement_ClutchBraking>();
    }

    
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, PointA.position, movementLogic.CurrentSpeed * Time.deltaTime);
            
    }
}
