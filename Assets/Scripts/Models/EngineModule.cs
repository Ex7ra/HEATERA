using UnityEngine;

public class EngineModule : TankModule
{
    public MonoBehaviour tankScript;
    private ITankMovement tank;

    void Awake()
    {
        tank = tankScript as ITankMovement;
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if (tank != null)
        {
            tank.EngineDestroyed();
            Debug.Log("Engine destroyed! Tank can't move."); //sir I have a problem
        }
    }
}
