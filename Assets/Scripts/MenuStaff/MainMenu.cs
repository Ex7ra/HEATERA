using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void GoToPoygonScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitTheGame()
    {
        Application.Quit();
        Debug.Log("Quit the game");
    }

}
