using UnityEngine;

public class AmmoRackModule : TankModule
{
    private TankDamageReceiver tankDamageReceiver;
    
    protected override void Awake()
    {
        base.Awake();
        tankDamageReceiver = GetComponentInParent<TankDamageReceiver>();
        
    }
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log("Ammo exploded! Tank destroyed.");
        if (tankDamageReceiver != null)
        {
            tankDamageReceiver.DestroyTank();
        }
        
    }
}
