using UnityEngine;

public class TurretTraverseModule : TankModule
{
    public TurretNormal turret;
    public AI_TurretController aiTurret;

    private void Update()
    {
        if (IsDestroyed)
        {
            if (turret != null)
            {
                turret.rotationMultiplier = 0f;
            }

            if (aiTurret != null)
            {
                aiTurret.rotationMultiplier = 0f;
            }
        }
        else if (health < maxHealth)
        {
            if (turret != null)
            {
                turret.rotationMultiplier = health / 100;
            }
            
            if (aiTurret != null)
            {
                aiTurret.rotationMultiplier = health / 100;
            }
        }
        else
        {
             if (turret != null)
            {
                turret.rotationMultiplier = 1.0f;
            }

            if (aiTurret != null)
            {
                aiTurret.rotationMultiplier = 1.0f;
            }
        }
    }

    public override void Damage(float dmg)
    {
        base.Damage(dmg);
    }
}