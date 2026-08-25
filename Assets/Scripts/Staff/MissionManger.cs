using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public int missionID;

    public void CompleteMission()
    {
        GameProgress.Instance.completedMissions[missionID] = true;
        
    }
}