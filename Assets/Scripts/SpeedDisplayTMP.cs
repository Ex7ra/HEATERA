using UnityEngine;
using TMPro;

public class SpeedDisplayTMP : MonoBehaviour
{
    public TankController tank;
    public TextMeshProUGUI speedText;
    [Tooltip("Show speed converted to km/h (m/s * 3.6)")]
    public bool showKmh = true;

    void Start()
    {
        if (tank == null)
            tank = FindObjectOfType<TankController>();

        if (speedText == null)
            speedText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (tank == null || speedText == null) return;

        float speed = tank.CurrentSpeed;
        if (showKmh) speed *= 3.6f;

        speedText.text = Mathf.RoundToInt(speed).ToString() + (showKmh ? " km/h" : " m/s");
    }
}
