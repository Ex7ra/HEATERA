using UnityEngine;
using TMPro;
using System.Linq;// Gives access to LINQ functions like OrderBy() for sorting arrays


[RequireComponent(typeof(Rigidbody2D))]// Makes sure this GameObject always has a Rigidbody2D component
public class Shell : MonoBehaviour
{
    [Header("Base Stats")]
    public float penetrationMm = 300f;
    public float damage = 40f;
    public float maxDistance = 30f;

    public enum ShellType
    {
        APFSDS,
        HEAT,
        HE
    }

    [Header("Shell Type")]
    public ShellType shellType;

    [Header("Explosion")]
    public float explosionRadius;
    public float explosionDamage;

    [Header("Angle Penetration Loss")]
    public AnimationCurve penetrationByAngle = new AnimationCurve(new Keyframe(0f, 1f),new Keyframe(30f, 0.9f),new Keyframe(45f, 0.75f),new Keyframe(60f, 0.55f),new Keyframe(75f, 0.35f),new Keyframe(85f, 0.2f));

    private Rigidbody2D rb;// Cached reference to the shell's Rigidbody2D so we don't search for it repeatedly

    private Vector2 lastPosition;// Stores the shell's previous position so we can raycast along the distance travelled each frame
    private Vector2 shellDirection;// Stores the shell's travel direction for armour angle and penetration calculations

    private Vector3 startPos;// Remembers where the shell was fired so we can destroy it after travelling its maximum range

    private float remainingPenetration;// Current penetration left after passing through armour or internal components
    private float remainingDamage;// Current damage remaining after penetrating multiple objects

    private bool shellDead;// Prevents the shell from processing more hits after it has been destroyed

    private int hullLayer;
    private int turretLayer;

    private string combinedMessage = "";

    public GameObject hitTextPrefab;
    public float lifeTimeText = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        hullLayer = LayerMask.NameToLayer("TankHull");
        turretLayer = LayerMask.NameToLayer("TankTurret");

        switch(shellType)
        {
            case ShellType.APFSDS:
            explosionRadius = 0;
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
        lastPosition = transform.position;
        shellDirection = rb.linearVelocity.normalized;
        remainingPenetration = penetrationMm;
        remainingDamage = damage;
    }
    public void Initialize(Vector2 direction)
    {
        shellDirection = direction.normalized;
    }
    void Update()
    {
        if(shellDead)
            return;

        CheckHits();

        if(Vector3.Distance(transform.position,startPos) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void CheckHits()
    {
        Vector2 currentPosition = transform.position;// Store the shell's current position this frame
        Vector2 travel = currentPosition - lastPosition;// Calculate how far and in what direction the shell moved since the previous frame

        if(travel.magnitude <= 0)// If the shell didn't move this frame, there is nothing to raycast
            return;

        // Cast a ray along the distance travelled this frame and collect every object it passes through
        RaycastHit2D[] hits = Physics2D.RaycastAll(lastPosition,travel.normalized,travel.magnitude);

        hits = hits.OrderBy(h => h.distance).ToArray();// Sort all raycast hits from closest to furthest so the shell interacts with objects in the correct order

        foreach(RaycastHit2D hit in hits)// Go through every object that the ray passed through, starting with the closest
        {

            if(hit.collider.gameObject == gameObject)// Ignore the shell's own collider if the ray detects it
                continue;

            ProcessHit(hit);// Send this hit to the penetration and damage system

            if(shellDead)// Stop checking more hits if the shell has already been destroyed
            break;
        }

        lastPosition = currentPosition;// Save the current position so it becomes the previous position on the next frame
    }

    void ProcessHit(RaycastHit2D hit)// Processes what happens after the shell hits an object (armour, module, etc)
    {
        if(!IsValidTarget(hit.collider.gameObject))// Ignore objects that are not part of the current firing mode
        return;

        TankArmor armor = hit.collider.GetComponent<TankArmor>();// Check if the object hit has an armour component

        if(armor != null)// If armour was hit, perform penetration calculations

        {   // Ignore armour that belongs to the wrong fire mode (hull or turret)
            if(TurretNormal.ActiveMode == TurretNormal.FireMode.HullOnly && hit.collider.gameObject.layer != hullLayer)
            {
                return;
            }
            // Ignore armour that belongs to the wrong fire mode (hull or turret)
            if(TurretNormal.ActiveMode == TurretNormal.FireMode.TurretOnly && hit.collider.gameObject.layer != turretLayer)
            {
                return;
            }

            float plateNormal;// Stores the direction angle of the armour surface
            float incoming;// Stores the direction from which the shell approaches the armour
            float impact;// Stores the angle at which the shell hits the armour
            bool ricochet;// Stores whether the shell ricocheted from the armour

            // Calculate effective armour thickness and get information about the impact angle and ricochet
            float effectiveArmor = armor.GetEffectiveArmorFromRay(shellDirection,hit.normal,out plateNormal,out incoming,out impact,out ricochet);
            // Calculate the shell's effective penetration after applying the armour impact angle penalty
            float currentPenetration = remainingPenetration * penetrationByAngle.Evaluate(impact);

            Debug.Log($"ARMOUR {armor.name} | Pen {currentPenetration} | Armor {effectiveArmor}");// Display penetration and armour values for testing

            if(ricochet || currentPenetration < effectiveArmor)// If the shell ricochets or lacks penetration, stop the projectile
            {
                combinedMessage += "Ricochet\n";
                ShowCombinedText();
                shellDead = true;
                Destroy(gameObject);
                return;
            }

            remainingPenetration -= effectiveArmor;// Remove the armour thickness from the shell's remaining penetration power
            Debug.Log("ARMOUR PENETRATED Remaining: " + remainingPenetration);

            if(shellType == ShellType.HEAT)// HEAT creates its damage effect after penetration and does not continue like APFSDS.
            {
                Explode();
                ShowCombinedText();
                shellDead = true;
                Destroy(gameObject);
                return;
            }

            return;
        }

        TankModule module = hit.collider.GetComponent<TankModule>();

        if(module != null)
        {
            float appliedDamage = remainingDamage;

            if(shellType == ShellType.APFSDS)
            {
                appliedDamage *= 1.2f;

                remainingDamage *= 0.7f;

                remainingPenetration *= 0.85f;
            }

            module.Damage(appliedDamage);
            combinedMessage += module.name + " Damaged\n";
            Debug.Log(module.name + " damaged " + appliedDamage);

            return;
        }

        if(shellType == ShellType.HE)
        {
            Explode();

            shellDead = true;
            Destroy(gameObject);
        }
    }

    void Explode()
    {

        if(shellType == ShellType.APFSDS)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,explosionRadius);

        foreach(Collider2D hit in hits)
        {
            TankModule module = hit.GetComponent<TankModule>();

            if(module == null)
                module = hit.GetComponentInParent<TankModule>();

            if(module != null)
            {
                float distance = Vector2.Distance(transform.position,hit.transform.position);
                float multiplier = Mathf.Clamp01(1f - distance / explosionRadius);
                float finalDamage = explosionDamage *multiplier;

                module.Damage(finalDamage);
                combinedMessage += module.name + " Damaged\n";
                Debug.Log("[Explosion] " + module.name + " " +finalDamage);
            }
        }
    }

    void ShowCombinedText()
    {
        if(string.IsNullOrEmpty(combinedMessage))
            return;
        Vector3 offset = new Vector3(0f,0.3f,0f);
        GameObject obj = Instantiate(hitTextPrefab,transform.position + offset,Quaternion.identity);
        TMP_Text text = obj.GetComponentInChildren<TMP_Text>();

        if(text != null)
        {
            text.text = combinedMessage.TrimEnd();
        }
        Destroy(obj,lifeTimeText);
        combinedMessage = "";
    }
    bool IsValidTarget(GameObject obj)
    {
        if(TurretNormal.ActiveMode == TurretNormal.FireMode.HullOnly)
        {
            return obj.layer == hullLayer;
        }


        if(TurretNormal.ActiveMode == TurretNormal.FireMode.TurretOnly)
        {
            return obj.layer == turretLayer;
        }

        return true;
    }
}