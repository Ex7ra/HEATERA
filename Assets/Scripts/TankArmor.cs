using UnityEngine;

public class TankArmor : MonoBehaviour
{
    [Header("Armor Settings")]
    public float plateEffArmour = 85f;

    [Range(0f, 90f)]
    [Tooltip("Set the angle above which the shell ricochets.")]
    public float ricochetAngleDeg = 90f; 

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
        
        Vector2 shellDir = shellVelocityDir.normalized;
        Vector2 incoming = -shellDir; 

       
        Vector2 normal = (normalAxis == NormalAxis.Up) ? transform.up : transform.right;
        if (flipNormal) normal = -normal;
        normal.Normalize();

      
        Debug.DrawRay(transform.position, normal, Color.green, 1f);
        Debug.DrawRay(transform.position, incoming, Color.red, 1f);

       
        plateNormalDeg = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        shellIncomingDeg = Mathf.Atan2(incoming.y, incoming.x) * Mathf.Rad2Deg;

       
        impactAngleDeg = Vector2.Angle(incoming, normal);

        
        impactAngleDeg = Mathf.Clamp(impactAngleDeg, 0.01f, 89.9f);

        
        ricochet = impactAngleDeg >= ricochetAngleDeg;

        
        float totalAngleRad = impactAngleDeg * Mathf.Deg2Rad;
        float effectiveArmor = plateEffArmour / Mathf.Cos(totalAngleRad);

        effectiveArmor = Mathf.Min(effectiveArmor, 500f);

        return effectiveArmor;
    }
}
