using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenus : MonoBehaviour
{

    public GameObject pauseMenu;
    public Mission_Task task;
    private bool isPaused = false;
   
    void Start()
    {
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        OpenPauseMenu();
    }
    public event System.Action OnPause;
    void OpenPauseMenu()
    {
        if (task == null || task.MissionFinished == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    OnPause?.Invoke();
                    isPaused = true;
                }
                else
                {
                    OnResume?.Invoke();
                    isPaused = false;
                }

                pauseMenu.SetActive(isPaused);
                Time.timeScale = isPaused ? 0 : 1;
            }
        }
    }
    public event System.Action OnResume;
    public void ResumeGame()
    {
        OnResume?.Invoke();
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToMenu()
    { 
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }
    

}
