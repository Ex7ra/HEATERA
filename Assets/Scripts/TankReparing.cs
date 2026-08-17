using UnityEngine;
using System.Collections;
public class TankReparing : MonoBehaviour
{
    
    public TankModule[] module;
    

    void Start()
    {
        module = GetComponentsInChildren<TankModule>();
        Debug.Log("Modules found: " + module.Length);
    }
    
    void Update()
    {
        foreach(TankModule currentModule in module)
        {
            float healthPercent = currentModule.health / currentModule.maxHealth;
            if (healthPercent <= 0.00f)
            {
                Debug.Log("PRESS R TO REPAIR: " + currentModule.name);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartCoroutine(RepairModule(currentModule));
                }
            }
        }
       
    }

    public IEnumerator RepairModule(TankModule currentModule)
    {
      
        float repairTime = currentModule.maxHealth / 15f;
        float time = 0f;
        while (time < repairTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / repairTime);
            print("Repairing " + currentModule.name + " " + (t * 100f).ToString("F2") + "%");
            yield return null;
        }
        if(time >= repairTime)
        {
            currentModule.health = currentModule.maxHealth;
            currentModule.IsDestroyed = false;
            Debug.Log(currentModule.name + " REPAIRED");
            
        }
        
    }
}
