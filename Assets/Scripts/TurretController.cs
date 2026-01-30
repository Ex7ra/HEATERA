using UnityEngine;
using System.Collections;

public class TurretNormal : MonoBehaviour
{
    public float rotationSpeed = 120f; 

    public Transform firePoint;      
    public GameObject shellPrefab;   
    public float shellSpeed = 12f;

    public float reloadTime = 3f;   
    private bool isReloading = false;

    public int totalShells = 30; 

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
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
        if (isReloading) return;
        if (totalShells <= 0) return; 

        if (Input.GetMouseButton(0))
        {
            Shoot();
            totalShells--; 
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (shellPrefab == null || firePoint == null) return;

        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = firePoint.up * shellSpeed;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
    }
}
