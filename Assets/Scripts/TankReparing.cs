using UnityEngine;
using System.Collections;
public class TankReparing : MonoBehaviour
{

    public TankModule module;
   
    void Start()
    {
        
    }

    
    void Update()
    {
        float healthPercent = (float)module.health / module.maxHealth;
        if(healthPercent == 0.00f)
        {
            Debug.Log("PRESS R TO REPAIR: " + module.name);
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(RepairModule());
            }
        }
    }

    public IEnumerator RepairModule()
    {
      
        float repairTime = module.maxHealth / 6f;
        float time = 0f;
        while (time < repairTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / repairTime);

            yield return null;
        }
        if(time >= repairTime)
        {
            module.health = module.maxHealth;
            module.IsDestroyed = false;
            Debug.Log(module.name + " REPAIRED");
        }
        
    }
}
