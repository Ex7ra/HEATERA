using UnityEngine;

public class CrewModule : TankModule
{   [Header("tank script turret/movement controller")]
    public TankController_ModernClutch ModernClutch;
    public TankController_ClutchBraking ClutchBraking;
    public TurretNormal Turret;
    public AI_Movement_ClutchBraking AI_ClutchBreaking;
    public AI_Movement_ModernClutch AI_ModernClutch;
    public AI_TurretController AI_Turret;
    public TankDamageReceiver damageReceiver;
    
    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Crew member killed: " + name);

        if (damageReceiver != null)
        {
            damageReceiver.CheckCrewStatus();
        }
    }
}