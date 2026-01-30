using UnityEngine;

public class TankArmor : MonoBehaviour
{
    public float thicknessMm = 85f;

    [Range(0f, 90f)]
    public float ricochetAngleDeg = 90f; // set 90 to disable

    public enum NormalAxis { Up, Right }
    public NormalAxis normalAxis = NormalAxis.Up;

    [Tooltip("Flip the normal direction if it's pointing inward.")]
    public bool flipNormal = false;

    public float GetEffectiveArmorFromVelocity(
        Vector2 shellVelocityDir,
        out float plateNormalDeg,
        out float shellIncomingDeg,
        out float impactAngleDeg,
        out bool ricochet)
    {
        Vector2 vel = shellVelocityDir.normalized;
        Vector2 incoming = (-vel).normalized; // direction INTO armor

        Vector2 normal = (normalAxis == NormalAxis.Up)
            ? ((Vector2)transform.up).normalized
            : ((Vector2)transform.right).normalized;

        if (flipNormal) normal = -normal;

        plateNormalDeg = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        shellIncomingDeg = Mathf.Atan2(incoming.y, incoming.x) * Mathf.Rad2Deg;

        impactAngleDeg = Vector2.Angle(incoming, normal);

        ricochet = false;
        
        float cos = Mathf.Abs(Vector2.Dot(incoming, normal));
        cos = Mathf.Max(cos, 0.01f);

        return thicknessMm / cos;
    }
}
