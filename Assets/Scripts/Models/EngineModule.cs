using UnityEngine;

public class EngineModule : TankModule
{
    public TankController tank; 

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if (tank != null)
        {
            tank.engineHealth = 0f; 
            Debug.Log("Engine destroyed! Tank can't move."); //sir I have i problam
        }
    }
}
