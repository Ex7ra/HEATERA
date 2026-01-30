using UnityEngine;

public class TankDamageReceiver : MonoBehaviour
{
    public TankController tank;

    public TankModule engine;
    public TankModule leftTrack;
    public TankModule rightTrack;
    public TankModule ammoRack;

    public TankModule[] crew;

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
    }
}
