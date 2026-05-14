using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class TurretNormal : MonoBehaviour
{
    public float rotationSpeed = 120f;//turret rotation speed(degrees per second)

    public Transform firePoint;//Fire point for spawning shells
    public float defaultRotationOffset = -180f;//this is used to correct the angle of the turret so it points towards the mouse correctly, it depends on how your turret sprite is oriented

    [Header("recoil Settings")]// recoil settings for the cannon
    public float recoilDistance = 0.3f;   
    public float recoilSpeed = 5f;        

    
     public enum ShellType//Note: enum is the list(in this case the list of shell types) and the values are the options in that list
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
    public bool gunOperational = true;//if its false it cant shoot
    public float rotationMultiplier = 1f;
    public float reloadMultiplier = 1f;//all those 3 lines makes something if its damages(NOT DESTROYED) it becomes slower so less effective but not completely useless
    public float accuracyMultiplier = 1f;

    

    [System.Serializable]//this lets Unity show this class in the inspector
    public class ShellData
    {
        public GameObject prefab;
        public float speed = 12f;
        public float reloadTime = 3f;
    }

    public ShellData AP;
    public ShellData HEAT;//those are configs for each shell type that you can set in the inspector
    public ShellData HE;
    public float switchShellTime = 6f;//takes time
    private bool isSwitchingShell = false;//blockes shooting while switching shells

    private bool isReloading = false;//blockes shooting while reloading
    public int totalShells = 30;//total shells copacity

    private Camera mainCam;//converts mouse position to world position

    public enum FireMode//this is the list of fire modes and the options in that list
    {
        HullOnly,
        TurretOnly
    }

    public static FireMode ActiveMode = FireMode.HullOnly;
    public float switchTime = 1.2f;
    private bool isSwitchingMode = false; // this makes switching modes take some time to actualy switch

    [Header("UI")]
    public TextMeshProUGUI modeText;//shows current mode
    public TextMeshProUGUI shellText;//shows current shell type
    public TextMeshProUGUI reloadingText;//shows reloading time

    private int shellLayer;
    private int hullLayer;//those 3 are used to control what collides with what
    private int turretLayer;

    void Start()
    {
        mainCam = Camera.main;//get the main camera

        shellLayer  = LayerMask.NameToLayer("Shell");//converts layer names to layer numbers so we can use them in the code
        hullLayer   = LayerMask.NameToLayer("TankHull");
        turretLayer = LayerMask.NameToLayer("TankTurret");

        ApplyLayerRules();//sets collision logic
        UpdateModeUI();
        UpdateShellUI();//UI texts
    }

    void Update()
    {
        RotateTurret();
        HandleShooting();
        HandleModeSwitch();
        HandleShellSwitch();
        //all those are system functions
    }

   
    void HandleShellSwitch()//shell switching input
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartShellSwitch(ShellType.AP);
       
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartShellSwitch(ShellType.HEAT);
     
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartShellSwitch(ShellType.HE);
       
    }
   IEnumerator RecoilCannon()//moves cannon backwords and the forward to simulate recoil when shooting, its a coroutine so it runs over time and not instant
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
    void StartShellSwitch(ShellType type)
    {
    if (isReloading || isSwitchingShell || currentShell == type) return;

    StartCoroutine(SwitchShell(type));
    }

    IEnumerator SwitchShell(ShellType type)//COROUTINE IMPORTANT CONCEPT:coroutine = function that runs over time
    {
    isSwitchingShell = true;

    float timer = switchShellTime;

    while (timer > 0f)//this system have countdown timer for swithcing shells
    {
        timer -= Time.deltaTime;

        if (shellText != null)
            shellText.text = "Switching: " + timer.ToString("F1");

        yield return null; 
    }

    currentShell = type;//Switch actualy happens here after the timer is done
    UpdateShellUI();

    Debug.Log("Loaded Shell → " + currentShell);

    isSwitchingShell = false;
    }

    ShellData GetCurrentShell()
    {
        return currentShell switch
        {
            ShellType.HEAT => HEAT,
            ShellType.HE => HE,
            _ => AP
        };
    }

    void HandleModeSwitch()
    {
        if (Input.GetMouseButtonDown(2) && !isSwitchingMode)//middle mouse button input for switching fire modes
        {
           StartCoroutine(SwitchFireMode());
            Debug.Log("Fire Mode → " + ActiveMode);
        }
    }

    void ApplyLayerRules()//colision logic for shells
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

        modeText.text = (ActiveMode == FireMode.HullOnly)// this is a ternary operator, its a shorter way of writing an if statement that assigns a value based on a condition
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
    Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);//converts mouse position to world position so we can rotate the turret towards it
    mouseWorld.z = 0f;

    Vector3 dir = mouseWorld - transform.position;//direction from turret to mouse position
    float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - defaultRotationOffset;//converts direction to angle in degrees

    float turretZ = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * rotationMultiplier * Time.deltaTime);//for smooth rotation
    transform.rotation = Quaternion.Euler(0, 0, turretZ);

  
   }

    
    void HandleShooting()
{
    
    if (!IsGunOperational())//stops everything if the gun is destroyed
        return;

    if (isReloading || isSwitchingMode || isSwitchingShell || totalShells <= 0 )
        return;

    if (Input.GetMouseButtonDown(0))
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
        Debug.Log("Gun disabled — module destroyed");
        gunOperational = false;
        return false;
    }
    

    gunOperational = true;
    return true;
  }

  void Shoot()
{
     ShellData shell = GetCurrentShell();

    GameObject obj = Instantiate(shell.prefab, firePoint.position, firePoint.rotation);//spawns shell prefab at fire point position and rotation
    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();//moves shell forward

    if (rb != null)
    {
        Vector2 fireDirection = firePoint.up;
        rb.linearVelocity = fireDirection * shell.speed;
    }

    StopCoroutine("RecoilCannon"); 
    StartCoroutine(RecoilCannon());//plays recoil animation when shooting
}

    IEnumerator Reload()
    {
    isReloading = true;

    float timer = GetCurrentShell().reloadTime * reloadMultiplier;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (reloadingText != null)
                reloadingText.text = "Reloading: " + timer.ToString("F1");

            yield return null;
        }

    if (reloadingText != null)
        reloadingText.text = "Ready"; 

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