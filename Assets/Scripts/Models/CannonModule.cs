using UnityEngine;

public class CannonModule : TankModule
{
    public TurretNormal turret;
    public  AI_TurretController aiTurret;

    public override void Damage(float dmg)
    {
        base.Damage(dmg);

        if (!IsDestroyed)
        {
            if(turret != null)
            {
                turret.accuracyMultiplier = 0.5f;
            }
            if(aiTurret != null)
            {
                aiTurret.accuracyMultiplier = 0.5f;  
            }
            
     
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        if(turret != null){
        turret.gunOperational = false;
        }
        if(aiTurret != null){
        aiTurret.gunOperational = false;
        }
        Debug.Log("Cannon Ruptured → Firing Locked");
    }
}