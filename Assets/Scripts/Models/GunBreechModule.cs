using UnityEngine;

public class GunBreechModule : TankModule
{
    public TurretNormal turret;

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        turret.gunOperational = false;

        Debug.Log("Breech Destroyed → Gun Disabled");
    }
}