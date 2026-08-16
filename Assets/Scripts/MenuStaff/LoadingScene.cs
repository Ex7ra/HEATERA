using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
   public TextMeshProUGUI LoadingText;
    void Start()
    {
        StartCoroutine(LoadLevel());

    }


   IEnumerator LoadLevel()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(MainMenu.sceneToLoad);
        while (!operation.isDone)
        {
            if(LoadingText != null)
                LoadingText.text = "<Loading> " + (operation.progress * 100f).ToString("F0") + "%";
            yield return null;
        }
    }
}
