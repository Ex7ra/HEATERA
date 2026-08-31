using UnityEngine;
using Pathfinding;

public class TankDamageReceiver : MonoBehaviour
{
   
    private ITankMovement tank;
    [Header("Sprite Renderers")]
    public SpriteRenderer hullRenderer;
    public SpriteRenderer turretRenderer;
    public SpriteRenderer cannonRenderer;

    
    [Header("DestroyedSpirtes")]
    public Sprite HullDestroyed;
    public Sprite TurretDestoyed;
    public Sprite CannonDestroyed;
    [Header("Crew")]
    public TankModule[] crew;

    public bool IsDestroyed { get; private set; }

    void Awake()
    {
        tank = GetComponent<ITankMovement>();
    }
   

    public void OnPenetration(Vector2 hitPoint, Vector2 direction, float damage)
    {
        if (IsDestroyed)
            return;

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
        if (IsDestroyed)
            return;

        int aliveCrew = 0;

        foreach (var member in crew)
        {
            if (member != null && !member.IsDestroyed)
            {
                aliveCrew++;
            }
        }

        Debug.Log(gameObject.name + " - Alive crew: " + aliveCrew);

        if (aliveCrew < 2)
        {
            DestroyTank();
        }
    }

    public void DestroyTank()
    {
        if (IsDestroyed)
            return;

        IsDestroyed = true;

        Debug.Log("Tank destroyed: not enough crew!");

        if (tank != null)
        {
            tank.DisableMovement();
        }

        ChangeSpritesToDestroyed();
        int obstacleLayer = LayerMask.NameToLayer("Obstacle");

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = obstacleLayer;
            child.gameObject.tag = "Destroyed Vechicle";
        }

        if (AstarPath.active != null)
        {
            AstarPath.active.Scan();
        }
    }
    void ChangeSpritesToDestroyed()
    {
        hullRenderer.sprite = HullDestroyed;
        turretRenderer.sprite = TurretDestoyed;
        cannonRenderer.sprite = CannonDestroyed;
    }
}