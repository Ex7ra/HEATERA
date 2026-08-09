using UnityEngine;

public class CrewModule : TankModule
{   [Header("tank script turret/movement controller")]
    public TankController_ModernClutch TankScript;
    public TankController_ClutchBraking TankScript_ClutchBraking;
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