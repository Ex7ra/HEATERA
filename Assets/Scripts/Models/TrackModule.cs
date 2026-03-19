using UnityEngine;

public class TrackModule : TankModule
{
    public TankController tank; 
    public bool isLeft;         

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        if (tank != null)
        {
            if (isLeft)
                tank.leftTrackHealth = 0f;
            else
                tank.rightTrackHealth = 0f;

            Debug.Log((isLeft ? "Left" : "Right") + " track destroyed!");
        }
    }
}
