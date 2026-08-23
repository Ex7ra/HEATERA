using UnityEngine;
using TMPro;

public class SpeedDisplayTMP : MonoBehaviour
{
    public MonoBehaviour tankScript; 
    private Tank tank;

    public TextMeshProUGUI speedText;
    public bool showKmh = true;

    void Awake()
    {
        tank = tankScript as Tank;

        if (speedText == null)
            speedText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (tank == null || speedText == null) return;

        float speed = tank.CurrentSpeed;
        if (showKmh) speed *= 3.6f;

        speedText.text = Mathf.RoundToInt(speed) + (showKmh ? " km/h" : " m/s");
    }
}