using UnityEngine;
using Pathfinding;

public class TankDamageReceiver : MonoBehaviour
{
    private ITankMovement tank;

    public TankModule[] crew;

    void Awake()
    {
        tank = GetComponent<ITankMovement>();
    }

    public void OnPenetration(Vector2 hitPoint, Vector2 direction, float damage)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(hitPoint, direction, 3f);

        foreach (var hit in hits)
        {
            TankModule module = hit.collider.GetComponent<TankModule>();
            if (module != null)
            {
                module.Damage(damage);
            }
        }

        CheckCrewStatus();
    }
    public void CheckCrewStatus()
    {
        int aliveCrew = 0;

        foreach (var member in crew)
        {
            if (member != null && !member.IsDestroyed)
                aliveCrew++;
        }

        if (aliveCrew < 2)
        {
            DestroyTank();
        }
    }

    void DestroyTank()
    {
        Debug.Log("Tank destroyed: not enough crew!");

        if (tank != null)
        {
            tank.DisableMovement();
        }

        int obstacleLayer = LayerMask.NameToLayer("Obstacle");

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = obstacleLayer;
            child.gameObject.tag = "Destroyed Vechicle";
        }

        AstarPath.active.Scan();
        
    }
}