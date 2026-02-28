using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class TurretNormal : MonoBehaviour
{
    public float rotationSpeed = 120f;

    public Transform firePoint;

    
     public enum ShellType
    {
        [Header(" SHELL TYPES")]
        AP,
        HEAT,
        HE
    }

    public ShellType currentShell = ShellType.AP;

    [System.Serializable]
    public class ShellData
    {
        public GameObject prefab;
        public float speed = 12f;
        public float reloadTime = 3f;
    }

    public ShellData AP;
    public ShellData HEAT;
    public ShellData HE;

    private bool isReloading = false;
    public int totalShells = 30;

    private Camera mainCam;

    public enum FireMode
    {
        HullOnly,
        TurretOnly
    }

    public static FireMode ActiveMode = FireMode.HullOnly;

    [Header("UI")]
    public TextMeshProUGUI modeText;
    public TextMeshProUGUI shellText;

    private int shellLayer;
    private int hullLayer;
    private int turretLayer;

    void Start()
    {
        mainCam = Camera.main;

        shellLayer  = LayerMask.NameToLayer("Shell");
        hullLayer   = LayerMask.NameToLayer("TankHull");
        turretLayer = LayerMask.NameToLayer("TankTurret");

        ApplyLayerRules();
        UpdateModeUI();
        UpdateShellUI();
    }

    void Update()
    {
        RotateTurret();
        HandleShooting();
        HandleModeSwitch();
        HandleShellSwitch();
    }

   
    void HandleShellSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetShell(ShellType.AP);
       
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetShell(ShellType.HEAT);
     
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetShell(ShellType.HE);
       
    }

    void SetShell(ShellType type)
    {
        if (isReloading) return; 

        currentShell = type;
        UpdateShellUI();

        Debug.Log("Loaded Shell → " + currentShell);
    }

    ShellData GetCurrentShell()
    {
        switch (currentShell)
        {
            case ShellType.HEAT: return HEAT;
            case ShellType.HE:   return HE;
            default:             return AP;
        }
    }

    void HandleModeSwitch()
    {
        if (Input.GetMouseButtonDown(2))
        {
            ActiveMode = (ActiveMode == FireMode.HullOnly)
                ? FireMode.TurretOnly
                : FireMode.HullOnly;

            ApplyLayerRules();
            UpdateModeUI();

            Debug.Log("Fire Mode → " + ActiveMode);
        }
    }

    void ApplyLayerRules()
    {
        if (ActiveMode == FireMode.HullOnly)
        {
            Physics2D.IgnoreLayerCollision(shellLayer, hullLayer, false);
            Physics2D.IgnoreLayerCollision(shellLayer, turretLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(shellLayer, turretLayer, false);
            Physics2D.IgnoreLayerCollision(shellLayer, hullLayer, true);
        }
    }

    void UpdateModeUI()
    {
        if (modeText == null) return;

        modeText.text = (ActiveMode == FireMode.HullOnly)
            ? "MODE: HULL"
            : "MODE: TURRET";
    }

    void UpdateShellUI()
    {
        if (shellText == null) return;

        shellText.text = "AMMO: " + currentShell.ToString();
    }

   
    void RotateTurret()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float newAngle = Mathf.MoveTowardsAngle(
            transform.eulerAngles.z,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    
    void HandleShooting()
    {
        if (isReloading || totalShells <= 0) return;

        if (Input.GetMouseButton(0))
        {
            Shoot();
            totalShells--;
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        ShellData shell = GetCurrentShell();

        GameObject obj = Instantiate(shell.prefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = firePoint.up * shell.speed;
    }

    IEnumerator Reload()
    {
        isReloading = true;

        float reload = GetCurrentShell().reloadTime;
        yield return new WaitForSeconds(reload);

        isReloading = false;
    }
}