using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject FSPanel;
    public GameObject NatoPanel;
    void Start()
    {
        FSPanel.SetActive(false);
        NatoPanel.SetActive(false);
    }
    public void OpenFSPanel()
    {
        FSPanel.SetActive(!FSPanel.activeSelf);
    }
    public void GoToPoygonScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitTheGame()
    {
        Application.Quit();
        Debug.Log("Quit the game");
    }
    public void OpenNatoPanel()
    {
       NatoPanel.SetActive(!NatoPanel.activeSelf); 
    }
    public void GoBack()
    {
        NatoPanel.SetActive(false);
    }
    
    public void PlayLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
    

}
