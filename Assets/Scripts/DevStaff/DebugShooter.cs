using UnityEngine;

public class DebugShooter : MonoBehaviour
{
    public GameObject shellPrefab;
    public float shellSpeed = 20f;

    [Header("Shooting Angle")]
    [Tooltip("0 = straight down, positive = right, negative = left")]
    public float shootAngleDegrees = 0f;

    [Header("Spawn")]
    public float spawnHeight = 5f;

    public enum ShootMode
    {
        Turret,
        Hull
    }

    [Header("Collision Mode")]
    public ShootMode shootMode = ShootMode.Turret;

    // Layer names (must match Unity layers)
    public string turretLayerName = "TankTurret";
    public string hullLayerName = "TankHull";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShootWithAngle();
        }
    }

    void ShootWithAngle()
    {
        // Target position = mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Spawn above the target
        Vector3 spawnPos = mouseWorld + Vector3.up * spawnHeight;

        // Base direction = straight down
        Vector2 baseDir = Vector2.down;

        // Rotate by angle
        Vector2 shootDir = Quaternion.Euler(0f, 0f, shootAngleDegrees) * baseDir;

        // Spawn rotation
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        // Instantiate shell
        GameObject shell = Instantiate(shellPrefab, spawnPos, rotation);

        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDir.normalized * shellSpeed;

        // --- Set shell collision layer based on mode ---
        int layer = (shootMode == ShootMode.Turret) ? LayerMask.NameToLayer(turretLayerName)
                                                   : LayerMask.NameToLayer(hullLayerName);
        shell.layer = layer;

        // Optional: Ignore collisions with other tank parts
        int otherLayer = (shootMode == ShootMode.Turret) ? LayerMask.NameToLayer(hullLayerName)
                                                         : LayerMask.NameToLayer(turretLayerName);

        Physics2D.IgnoreLayerCollision(shell.layer, otherLayer, true);
    }
}