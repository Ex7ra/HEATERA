using UnityEngine;

public class GunBreechModule : TankModule
{
    public TurretNormal turret;
    public AI_TurretController aiTurret;

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if(turret != null)
        {
            turret.gunOperational = false;
        }
        if(aiTurret != null)
        {
            aiTurret.gunOperational = false;
        }

        Debug.Log("Breech Destroyed → Gun Disabled");
    }
}