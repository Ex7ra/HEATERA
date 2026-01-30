using UnityEngine;

public class CrewModule : TankModule
{
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log("Crew member killed: " + name);
        // Later: connect to turret reload speed, aiming penalties, etc.
    }
}
