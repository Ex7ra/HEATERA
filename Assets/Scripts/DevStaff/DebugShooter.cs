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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShootWithAngle();
        }
    }

    void ShootWithAngle()
    {
        // Target position (mouse)
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0f;

        // Spawn above the target
        Vector3 spawnPos = targetPos + Vector3.up * spawnHeight;

        // Base direction = straight down
        Vector2 baseDir = Vector2.down;

        // Rotate direction by angle
        Vector2 shootDir = Quaternion.Euler(0f, 0f, shootAngleDegrees) * baseDir;

        // Calculate rotation so shell faces movement direction
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        // Spawn shell with correct rotation
        GameObject shell = Instantiate(shellPrefab, spawnPos, rotation);

        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDir.normalized * shellSpeed;
    }
}
