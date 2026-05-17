using UnityEngine;

public class TrackModule : TankModule
{
    public MonoBehaviour tankScript;
    private ITankMovement tank;

    public bool isLeft;

    void Awake()
    {
        tank = tankScript as ITankMovement;
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        if (tank != null)
        {
            if (isLeft)
                tank.DestroyLeftTrack();
            else
                tank.DestroyRightTrack();

            Debug.Log((isLeft ? "Left" : "Right") + " track destroyed!");
        }
    }
}