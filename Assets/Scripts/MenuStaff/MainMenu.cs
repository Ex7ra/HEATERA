using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject FSPanel;
    public GameObject NatoPanel;
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;

    private bool loadingVolume = true;

    void Start()
    {
        FSPanel.SetActive(false);
        NatoPanel.SetActive(false);
        float volume = 0.3f; 
        videoPlayer.SetDirectAudioVolume(0, volume);
        volumeSlider.value = volume;
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        videoPlayer.SetDirectAudioVolume(0, volume);
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