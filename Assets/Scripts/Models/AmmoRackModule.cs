using UnityEngine;

public class AmmoRackModule : TankModule
{
    private TankDamageReceiver tankDamageReceiver;
    void Awake()
    {
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
