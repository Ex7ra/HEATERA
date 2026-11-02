
using UnityEngine;

public class CannonModule : TankModule
{
    public TurretNormal turret;
    public AI_TurretController aiTurret;

    [Header("Sprites")]
    public SpriteRenderer cannonRenderer;
    public SpriteRenderer cannonShadowRenderer;
    public Sprite cannonDestroyedSprite;
    public Sprite cannonNormalSprite;

    public override void Damage(float dmg)
    {
        base.Damage(dmg);

        if (!IsDestroyed)
        {
            if (turret != null)
                turret.accuracyMultiplier = 0.5f;

            if (aiTurret != null)
                aiTurret.accuracyMultiplier = 0.5f;
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        if (turret != null)
            turret.gunOperational = false;

        if (aiTurret != null)
            aiTurret.gunOperational = false;

        
        cannonRenderer.sprite = cannonDestroyedSprite;
        cannonShadowRenderer.sprite = cannonDestroyedSprite;

    }

    protected override void OnRepaired()
    {
        base.OnRepaired();

        if (turret != null)
            turret.gunOperational = true;

        if (aiTurret != null)
            aiTurret.gunOperational = true;

        
        cannonRenderer.sprite = cannonNormalSprite;
        cannonShadowRenderer.sprite = cannonNormalSprite;

    }
}
