using UnityEngine;
using UnityEngine.UI;

public class UIModule : MonoBehaviour
{
    public TankModule module;
    public GameObject tank;
    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
    }

    void Update()
    {
        if(tank == null)
        {
            panelImage.color = new Color32(255, 61, 52, 255);
            return;
        }
        if (module == null)
            return;

        float healthPercent = (float)module.health / module.maxHealth;
        //Debug.Log("Health: " + module.health + " Max: " + module.maxHealth + " Percent: " + healthPercent);
        if (healthPercent > 0.80f)
        {
            panelImage.color = new Color32(118, 255, 95, 255);
        }
        else if (healthPercent > 0.55f)
        {
            panelImage.color = new Color32(255, 239, 95, 255);
        }
        else if (healthPercent > 0.15f)
        {
            panelImage.color = new Color32(255, 162, 52, 255);
        }
        else if (healthPercent == 0.00)
        {
            panelImage.color = new Color32(255, 61, 52, 255);
        }
        
        
        
    }
}