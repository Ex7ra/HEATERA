using UnityEngine;

public class TurretTraverseModule : TankModule
{
    public TurretNormal turret;
    public AI_TurretController aiTurret;
    public override void Damage(float dmg)
    {
        base.Damage(dmg);

        if (!IsDestroyed)
        {
            if(turret != null)
            {
                turret.rotationMultiplier = 0.4f; 
            }   
            if(aiTurret != null)
            {
                aiTurret.rotationMultiplier = 0.4f;
            }
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if(turret != null)
        {
        turret.rotationMultiplier = 0f;
        }
        if(aiTurret != null)
        {
        aiTurret.rotationMultiplier = 0f;
        }
        Debug.Log("Turret Jammed");
    }
}