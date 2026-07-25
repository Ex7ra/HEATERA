using UnityEngine;
using UnityEngine.UI;

public class UIModule : MonoBehaviour
{
    public TankModule module;

    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
    }

    void Update()
    {
        if (module == null)
            return;

        float healthPercent = (float)module.health / module.maxHealth;
        //Debug.Log("Health: " + module.health + " Max: " + module.maxHealth + " Percent: " + healthPercent);
        if (healthPercent > 0.80f)
        {
            panelImage.color = Color.green;
        }
        else if (healthPercent > 0.55f)
        {
            panelImage.color = Color.yellow;
        }
        else if (healthPercent > 0.15f)
        {
            panelImage.color = Color.orange;
        }
        else if (healthPercent > 0.01f)
        {
            panelImage.color = Color.red;
        }
        else if(healthPercent == 0.00000000f)
        {
            panelImage.color = Color.darkRed;
        }
        
    }
}