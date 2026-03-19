using UnityEngine;

public class CrewModule : TankModule
{
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log("Crew member killed: " + name);
    }
}
