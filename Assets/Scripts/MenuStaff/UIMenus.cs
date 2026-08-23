using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenus : MonoBehaviour
{

    public GameObject pauseMenu;
   
    void Start()
    {
        pauseMenu.SetActive(false);
        
    }
    void Update()
    {
        OpenPauseMenu();
        
    }
    void OpenPauseMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
    }
    public void ResumeGame()
    {
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
