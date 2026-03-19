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
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 spawnPos = mouseWorld + Vector3.up * spawnHeight;

        Vector2 baseDir = Vector2.down;

        Vector2 shootDir = Quaternion.Euler(0f, 0f, shootAngleDegrees) * baseDir;

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject shell = Instantiate(shellPrefab, spawnPos, rotation);

        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDir.normalized * shellSpeed;

       
        int layer = (shootMode == ShootMode.Turret) ? LayerMask.NameToLayer(turretLayerName)
        : LayerMask.NameToLayer(hullLayerName);
        shell.layer = layer;

        
        int otherLayer = (shootMode == ShootMode.Turret) ? LayerMask.NameToLayer(hullLayerName)
        : LayerMask.NameToLayer(turretLayerName);

        Physics2D.IgnoreLayerCollision(shell.layer, otherLayer, true);
    }
}