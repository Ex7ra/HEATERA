using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class TurretNormal : MonoBehaviour
{
    public float rotationSpeed = 120f;

    public Transform firePoint;

    [Header("recoil Settings")]
    public float recoilDistance = 0.3f;   
    public float recoilSpeed = 5f;        

    
     public enum ShellType
    {
        [Header(" SHELL TYPES")]
        AP,
        HEAT,
        HE
    }
    [Header("Cannon")]
    public Transform cannon;     
    public ShellType currentShell = ShellType.AP;

    [Header("Gun Modules")]
    public CannonModule cannonModule;
    public GunBreechModule gunBreechModule;

    [Header("Damage Effects")]
    public bool gunOperational = true;
    public float rotationMultiplier = 1f;
    public float reloadMultiplier = 1f;
    public float accuracyMultiplier = 1f;

    

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
    public float switchTime = 1.2f;
    private bool isSwitchingMode = false; // this makes switching modes take some time to actualy switch

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
   IEnumerator RecoilCannon()
   {
    Vector3 originalPos = cannon.localPosition;

    Vector3 recoilDirection = cannon.InverseTransformDirection(firePoint.up); 
    Vector3 recoilPos = originalPos - recoilDirection * recoilDistance;

    while (Vector3.Distance(cannon.localPosition, recoilPos) > 0.001f)
    {
        cannon.localPosition = Vector3.MoveTowards(cannon.localPosition, recoilPos, recoilSpeed * Time.deltaTime);
        yield return null;
    }

    while (Vector3.Distance(cannon.localPosition, originalPos) > 0.001f)
    {
        cannon.localPosition = Vector3.MoveTowards(cannon.localPosition, originalPos, recoilSpeed * Time.deltaTime);
        yield return null;
    }
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
        if (Input.GetMouseButtonDown(2) && !isSwitchingMode)
        {
           StartCoroutine(SwitchFireMode());
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
    Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
    mouseWorld.z = 0f;

    Vector3 dir = mouseWorld - transform.position;
    float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    float turretZ = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * rotationMultiplier * Time.deltaTime);
    transform.rotation = Quaternion.Euler(0, 0, turretZ);

    Vector3 fireDir = (mouseWorld - cannon.position).normalized;

  
   }

    
    void HandleShooting()
{
    
    if (!gunOperational)
        return;

    if (isReloading || totalShells <= 0 || isSwitchingMode)
        return;

    if (Input.GetMouseButton(0))
    {
        Shoot();
        totalShells--;
        StartCoroutine(Reload());
    }
}
   bool IsGunOperational()
  {
    if (cannonModule == null || gunBreechModule == null)
        return true; 

    if (cannonModule.IsDestroyed || gunBreechModule.IsDestroyed)
    {
        gunOperational = false;
        return false;
    }
    Debug.Log("Gun disabled — module destroyed");

    gunOperational = true;
    return true;
  }

  void Shoot()
{
     ShellData shell = GetCurrentShell();

    GameObject obj = Instantiate(shell.prefab, firePoint.position, firePoint.rotation);
    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

    if (rb != null)
    {
        Vector2 fireDirection = firePoint.up;
        rb.linearVelocity = fireDirection * shell.speed;
    }

    StopCoroutine("RecoilCannon"); 
    StartCoroutine("RecoilCannon");
}

    IEnumerator Reload()
    {
        isReloading = true;

        float reload = GetCurrentShell().reloadTime * reloadMultiplier;
        yield return new WaitForSeconds(reload);

        isReloading = false;
    }

    IEnumerator SwitchFireMode()
    {
        isSwitchingMode = true;
        float timer = switchTime;
        while (timer > 0f)
        {
            timer -=Time.deltaTime;
            if(modeText != null)
                modeText.text = "Switching: " + timer.ToString("F1");
            yield return null;
        }
        ActiveMode = (ActiveMode == FireMode.HullOnly)
        ? FireMode.TurretOnly
        : FireMode.HullOnly;

        ApplyLayerRules();
        UpdateModeUI();
        isSwitchingMode = false;
        
    }
}