using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Shell : MonoBehaviour
{
    public float penetrationMm = 300f;
    public float damage = 40f;
    public float maxDistance = 30f;

    private Vector3 startPos;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, startPos) > maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TankArmor armor = other.GetComponent<TankArmor>();
        if (armor == null) return;

        if (rb.linearVelocity.sqrMagnitude < 0.001f)
        {
            Debug.LogWarning("[HIT] Shell velocity too low at impact.");
            Destroy(gameObject);
            return;
        }

        float plateNormalDeg, incomingDeg, impactDeg;
        bool ricochet;

        float effectiveArmor = armor.GetEffectiveArmorFromVelocity(
            rb.linearVelocity,
            out plateNormalDeg,
            out incomingDeg,
            out impactDeg,
            out ricochet
        );

        bool penetrated = !ricochet && penetrationMm >= effectiveArmor;

        PenDebug.LogHit(
            plateName: other.name,
            penMm: penetrationMm,
            baseMm: armor.thicknessMm,
            plateNormalDeg: plateNormalDeg,
            incomingDeg: incomingDeg,
            impactDeg: impactDeg,
            effMm: effectiveArmor,
            ricochet: ricochet,
            penetrated: penetrated
        );

        if (penetrated)
        {
            ApplyDirectInternalDamage(other);
        }

        Destroy(gameObject);
    }

    void ApplyDirectInternalDamage(Collider2D armorCollider)
    {
        // Find ALL TankModules inside the tank
        TankModule[] modules = FindObjectsOfType<TankModule>();

        TankModule closest = null;
        float bestDist = float.MaxValue;
        Vector2 hitPoint = transform.position;

        foreach (var module in modules)
        {
            float dist = Vector2.Distance(hitPoint, module.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = module;
            }
        }

        if (closest != null)
        {
            closest.Damage(damage);
            Debug.Log($"[INTERNAL HIT] Module={closest.name} Damage={damage}");
        }
    }
}
