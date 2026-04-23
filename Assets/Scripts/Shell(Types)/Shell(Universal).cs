using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Shell : MonoBehaviour
{

    [Header("Base Stats")]
    public float penetrationMm = 300f;
    public float damage = 40f;
    public float maxDistance = 30f;

    public enum ShellType { APFSDS, HEAT, HE }
    [Header("Shell Types")]
    public ShellType shellType;
    [Header("Explosion Properties")]
    public float explosionRadius = 0f;
    public float explosionDamage = 0f;


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
        switch (shellType)
        {
            case ShellType.APFSDS:
                explosionRadius = 0f;
                break;
            case ShellType.HEAT:
                explosionRadius = 2f;
                break;
            case ShellType.HE:
                explosionRadius = 4f;
                break;

        }
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
    void Explode()
    {
        if (shellType == ShellType.APFSDS) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            TankModule module = hit.GetComponent<TankModule>();
            if (module == null)
                module = hit.GetComponentInParent<TankModule>();
            if (module != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                float finalDamage = explosionDamage * damageMultiplier;
                module.Damage(finalDamage);
                Debug.Log($"[EXPLOSION HIT] {module.name} took {finalDamage} ");
            }

                
            
        }
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (rb.linearVelocity.sqrMagnitude < 0.001f)
        {
            if(shellType != ShellType.APFSDS)
            {
            Explode();
            }
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
                float appliedDamage = damage;
                if (shellType == ShellType.APFSDS)
                {
                    appliedDamage *= 1.2f;
                }
                module.Damage(appliedDamage);
                Debug.Log($"[MODULE HIT] {module.name} took {appliedDamage} damage");

                
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
                if (shellType == ShellType.APFSDS)
                {
                    penetrationMm *= 0.85f;
                }
                else{
                hasPenetrated = true;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = false;
                Explode();
                }
               
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
