using UnityEngine;

public class TankArmor : MonoBehaviour
{
    [Header("Armor Settings")]
    public float plateEffArmour = 85f;

    [Range(0f, 90f)]
    public float ricochetAngleDeg = 75f;


    public float GetEffectiveArmorFromRay(
        Vector2 shellDirection,
        Vector2 hitNormal,
        out float plateNormalDeg,
        out float shellIncomingDeg,
        out float impactAngleDeg,
        out bool ricochet)
    {
        // Direction the shell is coming from
        Vector2 incoming = -shellDirection.normalized;

        // Unity gives us the real surface normal
        Vector2 normal = hitNormal.normalized;


        Debug.DrawRay(transform.position, normal, Color.green, 1f);
        Debug.DrawRay(transform.position, incoming, Color.red, 1f);


        plateNormalDeg = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        shellIncomingDeg = Mathf.Atan2(incoming.y, incoming.x) * Mathf.Rad2Deg;


        impactAngleDeg = Vector2.Angle(incoming, normal);

        impactAngleDeg = Mathf.Clamp(impactAngleDeg, 0f, 89.9f);


        ricochet = impactAngleDeg >= ricochetAngleDeg;


        float angleRad = impactAngleDeg * Mathf.Deg2Rad;

        float effectiveArmor = plateEffArmour / Mathf.Cos(angleRad);

        effectiveArmor = Mathf.Min(effectiveArmor, 500f);


        return effectiveArmor;
    }
}