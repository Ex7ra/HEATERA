using UnityEngine;

public class Mission_Task : MonoBehaviour
{
    public GameObject[] EnemyTanks;
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
    void Update()
    {
        CheckEnemyTanks();
        Debug.Log("Alive Enemy Tanks: " + aliveTanks);
    }
}
