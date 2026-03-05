using UnityEngine;

public class CannonModule : TankModule
{
    public TurretNormal turret;

    public override void Damage(float dmg)
    {
        base.Damage(dmg);

        if (!IsDestroyed)
        {
            turret.accuracyMultiplier = 0.5f;   // worse accuracy
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        turret.gunOperational = false;

        Debug.Log("Cannon Ruptured → Firing Locked");
    }
}