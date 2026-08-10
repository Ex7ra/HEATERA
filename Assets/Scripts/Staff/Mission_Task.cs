using UnityEngine;
using TMPro;
public class Mission_Task : MonoBehaviour
{
    public GameObject[] EnemyTanks;
    public TextMeshProUGUI aliveTanksText;
    private int aliveTanks;

    private void CheckEnemyTanks()
    {
        aliveTanks = 0;

        foreach (GameObject tankObject in EnemyTanks)
        {
            if (tankObject == null)
                continue;

            TankDamageReceiver damageReceiver =
                tankObject.GetComponent<TankDamageReceiver>();

            if (damageReceiver != null && !damageReceiver.IsDestroyed)
            {
                aliveTanks++;
            }
        }
    }
    void Awake()
    {
        if (aliveTanksText == null)
            aliveTanksText = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        CheckEnemyTanks();
        aliveTanksText.text = "Active enemy tanks: " + aliveTanks + "/" + EnemyTanks.Length;
    }
}
