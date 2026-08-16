using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PanelDirection
{
    Left,
    Right,
    Top,
    Bottom
}
[Serializable]
public class MenuPanel
{
    public string id;
    public RectTransform rectTransform;
    public PanelDirection direction = PanelDirection.Left;

    [Tooltip("If true, other open panels will be closed automatically when this one opens.")]
    public bool closeOthersOnOpen = true;

    [NonSerialized] public Vector2 startPosition;
    [NonSerialized] public bool isOpen;
    [NonSerialized] public Coroutine runningAnimation;
    [NonSerialized] public CanvasGroup canvasGroup;
}

public class MainMenu : MonoBehaviour
{
    public static string sceneToLoad;
    [Header("Panels")]
    public List<MenuPanel> panels = new List<MenuPanel>();

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;

    [Header("Panel Animation")]
    public float panelAnimationDuration = 0.4f;
    public float panelStartOffset = 1000f;

    private Dictionary<string, MenuPanel> panelLookup = new Dictionary<string, MenuPanel>();

    void Start()
    {
        foreach (var p in panels)
        {
            if (p.rectTransform == null)
            {
                Debug.LogWarning($"MainMenu: panel '{p.id}' has no RectTransform assigned.");
                continue;
            }

            p.startPosition = p.rectTransform.anchoredPosition;
            p.isOpen = false;
            p.canvasGroup = p.rectTransform.GetComponent<CanvasGroup>();
            if (p.canvasGroup == null)
                p.canvasGroup = p.rectTransform.gameObject.AddComponent<CanvasGroup>();

            p.rectTransform.gameObject.SetActive(false);

            if (!panelLookup.ContainsKey(p.id))
                panelLookup.Add(p.id, p);
            else
                Debug.LogWarning($"MainMenu: duplicate panel id '{p.id}'.");
        }

        float volume = 0.3f;
        if (videoPlayer != null)
            videoPlayer.SetDirectAudioVolume(0, volume);
        if (volumeSlider != null)
            volumeSlider.value = volume;
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (videoPlayer != null)
            videoPlayer.SetDirectAudioVolume(0, volume);
    }

    public void GoToPoygonScene()
    {
        sceneToLoad = "SampleScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitTheGame()
    {
        Application.Quit();
        Debug.Log("Quit the game");
    }

    public void PlayLevel(string sceneName)
    {
        sceneToLoad = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenPanel(string id)
    {
        if (!panelLookup.TryGetValue(id, out var panel))
        {
            Debug.LogWarning($"MainMenu: no panel registered with id '{id}'.");
            return;
        }

        if (panel.closeOthersOnOpen)
        {
            foreach (var other in panels)
            {
                if (other != panel && other.isOpen)
                    ClosePanelInternal(other);
            }
        }

        if (panel.isOpen) return;

        panel.isOpen = true;

        if (panel.runningAnimation != null)
            StopCoroutine(panel.runningAnimation);

        panel.rectTransform.gameObject.SetActive(true);
        panel.runningAnimation = StartCoroutine(AnimatePanel(panel, true));
    }

    public void ClosePanel(string id)
    {
        if (!panelLookup.TryGetValue(id, out var panel))
        {
            Debug.LogWarning($"MainMenu: no panel registered with id '{id}'.");
            return;
        }

        ClosePanelInternal(panel);
    }

    private void ClosePanelInternal(MenuPanel panel)
    {
        if (!panel.isOpen) return;

        panel.isOpen = false;

        if (panel.runningAnimation != null)
            StopCoroutine(panel.runningAnimation);

        panel.runningAnimation = StartCoroutine(AnimatePanel(panel, false));
    }

    private IEnumerator AnimatePanel(MenuPanel panel, bool opening)
    {
        RectTransform rect = panel.rectTransform;
        Vector2 offset = GetOffset(panel.direction);

        Vector2 startPos = opening ? panel.startPosition + offset : rect.anchoredPosition;
        Vector2 targetPos = opening ? panel.startPosition : panel.startPosition + offset;

        rect.anchoredPosition = startPos;

   
        panel.canvasGroup.blocksRaycasts = false;
        panel.canvasGroup.interactable = false;

        float time = 0f;
        while (time < panelAnimationDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / panelAnimationDuration);
            t = 1 - Mathf.Pow(1 - t, 3); // ease-out cubic

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos;

        if (opening)
        {
            panel.canvasGroup.blocksRaycasts = true;
            panel.canvasGroup.interactable = true;
        }
        else
        {
            rect.gameObject.SetActive(false);
            rect.anchoredPosition = panel.startPosition;
        }

        panel.runningAnimation = null;
    }

    private Vector2 GetOffset(PanelDirection direction)
    {
        switch (direction)
        {
            case PanelDirection.Right: return new Vector2(panelStartOffset, 0);
            case PanelDirection.Left: return new Vector2(-panelStartOffset, 0);
            case PanelDirection.Top: return new Vector2(0, panelStartOffset);
            case PanelDirection.Bottom: return new Vector2(0, -panelStartOffset);
            default: return Vector2.zero;
        }
    }
}