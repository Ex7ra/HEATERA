using UnityEngine;

public class CrewModule : TankModule
{
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