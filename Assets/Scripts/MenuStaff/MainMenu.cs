using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject FSPanel;
    public GameObject NatoPanel;
    public GameObject Level_1_Panel_NATO;
    public GameObject Level_1_Panel_WarsawPart;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;

    void Start()
    {
        FSPanel.SetActive(false);
        NatoPanel.SetActive(false);
        Level_1_Panel_NATO.SetActive(false);
        Level_1_Panel_WarsawPart.SetActive(false);
        float volume = 0.3f; 
        videoPlayer.SetDirectAudioVolume(0, volume);
        volumeSlider.value = volume;
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        videoPlayer.SetDirectAudioVolume(0, volume);
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
    public void SetPanelActive(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void GoBack(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void PlayLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
    
}