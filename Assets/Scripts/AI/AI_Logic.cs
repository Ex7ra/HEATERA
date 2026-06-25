using UnityEngine;

public class AI_Logic : MonoBehaviour
{
    
    private AI_Movement_ClutchBraking movementLogic;
    public GameObject PointA;
    
    void Start()
    {
        movementLogic = GetComponent<AI_Movement_ClutchBraking>();
    }

    
    void Update()
    {
        
            
    }
}
