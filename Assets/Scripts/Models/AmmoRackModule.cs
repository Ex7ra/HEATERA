using UnityEngine;

public class AmmoRackModule : TankModule
{
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log("Ammo exploded! Tank destroyed.");
        // Destroy entire tank
        Destroy(transform.root.gameObject);
    }
}
