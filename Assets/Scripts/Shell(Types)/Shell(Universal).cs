using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Shell : MonoBehaviour
{
    [Header("Base Stats")]
    public float penetrationMm = 300f;
    public float damage = 40f;
    public float maxDistance = 30f;

    [Header("Angle Penetration Loss")]
    public AnimationCurve penetrationByAngle = new AnimationCurve(
        new Keyframe(0f, 1.0f),
        new Keyframe(30f, 0.9f),
        new Keyframe(45f, 0.75f),
        new Keyframe(60f, 0.55f),
        new Keyframe(75f, 0.35f),
        new Keyframe(85f, 0.2f)
    );

    private Vector3 startPos;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasPenetrated = false;

    
    private int hullLayer;
    private int turretLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hullLayer = LayerMask.NameToLayer("TankHull");
        turretLayer = LayerMask.NameToLayer("TankTurret");

        if (hullLayer == -1 || turretLayer == -1)
            Debug.LogError("TankHull or TankTurret layer not found! Please create them in Tags & Layers.");
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
        if (rb.linearVelocity.sqrMagnitude < 0.001f)
        {
            Destroy(gameObject);
            return;
        }

        
        TankModule module = other.GetComponent<TankModule>();
        if (module == null)
            module = other.GetComponentInParent<TankModule>();

        bool validModule = false;
        if (module != null)
        {
            if (TurretNormal.ActiveMode == TurretNormal.FireMode.HullOnly && other.gameObject.layer == hullLayer)
                validModule = true;
            if (TurretNormal.ActiveMode == TurretNormal.FireMode.TurretOnly && other.gameObject.layer == turretLayer)
                validModule = true;

            if (validModule)
            {
                module.Damage(damage);
                Debug.Log($"[MODULE HIT] {module.name} took {damage} damage");

                
                if (!hasPenetrated)
                {
                    hasPenetrated = true;
                    if (spriteRenderer != null)
                        spriteRenderer.enabled = false; 
                }

               
                return;
            }
        }

        
        TankArmor armor = other.GetComponent<TankArmor>();
        if (armor != null)
        {
           
            if (TurretNormal.ActiveMode == TurretNormal.FireMode.HullOnly && other.gameObject.layer != hullLayer)
                return;
            if (TurretNormal.ActiveMode == TurretNormal.FireMode.TurretOnly && other.gameObject.layer != turretLayer)
                return;

            float plateNormalDeg, incomingDeg, impactDeg;
            bool ricochet;

            float effectiveArmor = armor.GetEffectiveArmorFromVelocity(
                rb.linearVelocity,
                out plateNormalDeg,
                out incomingDeg,
                out impactDeg,
                out ricochet
            );

            impactDeg = Mathf.Clamp(impactDeg, 0f, 85f);
            float angleMultiplier = penetrationByAngle.Evaluate(impactDeg);
            float effectivePenetration = penetrationMm * angleMultiplier;

            bool penetrated = !ricochet && effectivePenetration >= effectiveArmor;

           
            PenDebug.LogHit(
                other.name,
                effectivePenetration,
                armor.plateEffArmour,
                plateNormalDeg,
                incomingDeg,
                impactDeg,
                effectiveArmor,
                ricochet,
                penetrated
            );

            if (penetrated)
            {
                hasPenetrated = true;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = false;

               
                return;
            }
            else
            {
               
                Destroy(gameObject);
                return;
            }
        }
    }
}
