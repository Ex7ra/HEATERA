using UnityEngine;
using System.Collections;
using TMPro;    

public class TankReparing : MonoBehaviour
{
    public TankModule[] module;

    private TankDamageReceiver tank;

    [Header("Repair UI")]
    public GameObject repairUI;
    public TMP_Text repairText;
    public TMP_Text progressText;

    private bool isRepairing = false;
    private TankModule currentRepairModule;
    void Start()
    {
        module = GetComponentsInChildren<TankModule>();
        tank = GetComponent<TankDamageReceiver>();
        Debug.Log("Modules found: " + module.Length);
    }
    
    void Update()
    {
        if (tank.IsDestroyed)
        {
            repairUI.SetActive(false);
            return;
        }

        if (isRepairing)
        {
            return;
        }

        TankModule brokenModule = FindBrokenModule();

        if (brokenModule == null)
        {
            repairUI.SetActive(false);
            return;
        }

        repairUI.SetActive(true);

        repairText.text = brokenModule.name.ToUpper() + " DESTROYED\n\n" + "Press R to repair " + brokenModule.name;

        progressText.text = "";

        
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRepairModule = brokenModule;
            StartCoroutine(RepairModule(currentRepairModule));
        }
    }

    public IEnumerator RepairModule(TankModule currentModule)
    {
        isRepairing = true;
        float repairTime = currentModule.maxHealth / 15f;
        float time = 0f;

        repairUI.SetActive(true);

        while (time < repairTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / repairTime);

            repairText.text = "REPAIRING " + currentModule.name.ToUpper();
            progressText.text = "Repairing: " +(t * 100f).ToString("F0") +"%";

            yield return null;
        }

        currentModule.health = currentModule.maxHealth;
        currentModule.IsDestroyed = false;
        Debug.Log(currentModule.name + " REPAIRED");

        isRepairing = false;
    }
    TankModule FindBrokenModule()
    {
        foreach (TankModule currentModule in module)
        {
            float healthPercent =
                currentModule.health / currentModule.maxHealth;

            if (healthPercent <= 0f)
            {
                return currentModule;
            }
        }

        return null;
    }
}
