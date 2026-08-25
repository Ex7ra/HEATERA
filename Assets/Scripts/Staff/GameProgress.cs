using UnityEngine;

public class GameProgress : MonoBehaviour
{
   public static GameProgress Instance;

   public bool[] completedMissions;

   void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            completedMissions = new bool[10];

        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int GetCompletedMissionCount()
    {
        int count = 0;

        foreach (bool completed in completedMissions)
        {
            if (completed)
            {
                count++;
            }
        }

        return count;
    }
}
