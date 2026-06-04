using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject FSPanel;
    void Start()
    {
        FSPanel.SetActive(false);
    }
    public void OnPoinetrDown()
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

}
