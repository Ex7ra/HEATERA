using UnityEngine;
using System;
public interface TankTurret
{
    float CurrentTurretRotation { get; }

    bool IsReloading { get; }

    event Action OnShoot;
    event Action OnReload;
}
