using UnityEngine;

public class AmmoRackModule : TankModule
{
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log("Ammo exploded! Tank destroyed.");
        
        Destroy(transform.root.gameObject);// I imaging in my imagination what tank has brutaly exploded
    }
}
