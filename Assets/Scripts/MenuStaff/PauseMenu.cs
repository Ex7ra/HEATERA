using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    
    public GameObject pauseMenu;
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void OpenPauseMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
   
}
