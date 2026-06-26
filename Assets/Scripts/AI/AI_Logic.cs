using UnityEngine;

public class AI_Logic : MonoBehaviour
{
    private AI_Movement_ModernClutch movementLogic;
    public Transform PointA;

    void Start()
    {
        movementLogic = GetComponent<AI_Movement_ModernClutch>();
    }

    void Update()
    {
        if (movementLogic == null || PointA == null)
            return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = PointA.position;
        Vector2 direction = targetPosition - currentPosition;
        float distance = direction.magnitude;

        if (distance < 0.1f)
        {
            movementLogic.moveInput = 0f;
            movementLogic.turnInput = 0f;
            return;
        }

        direction /= distance;
        Vector2 forward = (Vector2)transform.up;

        float angleToTarget = Vector2.SignedAngle(forward, direction);
        float turnInput = Mathf.Clamp(angleToTarget / 90f, -1f, 1f);
        float moveInput = (distance > 0.5f && Mathf.Abs(angleToTarget) < 25f) ? 1f : 0f;

        movementLogic.moveInput = moveInput;
        movementLogic.turnInput = turnInput;
    }
}
