using UnityEngine;

public class EngineModule : TankModule
{
    public TankController tank; // assign in inspector

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if (tank != null)
        {
            tank.engineHealth = 0f; // engine destroyed → tank stops
            Debug.Log("Engine destroyed! Tank can't move.");
        }
    }
}
