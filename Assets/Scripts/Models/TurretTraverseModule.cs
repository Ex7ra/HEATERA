using UnityEngine;

public class TurretTraverseModule : TankModule
{
    public TurretNormal turret;

    public override void Damage(float dmg)
    {
        base.Damage(dmg);

        if (!IsDestroyed)
        {
            turret.rotationMultiplier = 0.4f; // slow traverse
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        turret.rotationMultiplier = 0f;

        Debug.Log("Turret Jammed");
    }
}