using UnityEngine;
using System.Collections;

public class TurretMagazine : MonoBehaviour
{
    public float rotationSpeed = 120f; // degrees per second

    public Transform firePoint;      
    public GameObject shellPrefab;   
    public float shellSpeed = 12f;

    public float fireRate = 4f;      // shots per second inside the magazine
    public int magazineSize = 5;     // shells before reload
    public float reloadTime = 3f;    // full-mag reload

    public int totalShells = 30;     // total shells the tank carries

    private float fireCooldown;
    private int shellsLeft;
    private bool isReloading = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        shellsLeft = magazineSize;
    }

    void Update()
    {
        RotateTurret();
        HandleShooting();
    }

    void RotateTurret()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    void HandleShooting()
    {
        if (isReloading || totalShells <= 0)
        {
            fireCooldown -= Time.deltaTime;

            if (isReloading && fireCooldown <= 0f)
            {
                shellsLeft = Mathf.Min(magazineSize, totalShells); // refill magazine
                isReloading = false;
            }
            return;
        }

        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            Shoot();
            shellsLeft--;
            totalShells--;
            fireCooldown = 1f / fireRate;

            if (shellsLeft <= 0 && totalShells > 0)
            {
                isReloading = true;
                fireCooldown = reloadTime;
            }
        }
    }

    void Shoot()
    {
        if (shellPrefab == null || firePoint == null) return;

        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = firePoint.up * shellSpeed;
    }
}
