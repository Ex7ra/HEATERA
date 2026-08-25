using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
public class Mission_Task : MonoBehaviour
{
    public Canvas canvas;
    public MissionManager missionManager;
    public GameObject[] EnemyTanks;
    public TextMeshProUGUI aliveTanksText;
    public  GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public float panelAppearTime = 1f;
    public float colourTransitionTime = 0.5f;

    public Color normalColour = new Color(125f / 255f, 255f / 255f, 0f / 255f);
    public Color defeatColour = new Color(255f / 255f, 0f / 255f, 10f / 255f);
    public Color victoryColour = new Color(0f / 255f, 58f / 255f, 255f / 255);
    private int aliveTanks;
    public TankDamageReceiver PlayerTank;
    public bool MissionFinished = false;
    public GameObject EndingMenuPanel;

    private void CheckEnemyTanks()
    {
        aliveTanks = 0;

        foreach (GameObject tankObject in EnemyTanks)
        {
            if (tankObject == null)
                continue;

            TankDamageReceiver damageReceiver = tankObject.GetComponent<TankDamageReceiver>();

            if (damageReceiver != null && !damageReceiver.IsDestroyed)
            {
                aliveTanks++;
            }
            
        }
        if (aliveTanks == 0 && !MissionFinished)//WIN
        {
            VictoryPanel.SetActive(true);
            ShowPanel(VictoryPanel, victoryColour);
            MissionFinished = true;
            missionManager.CompleteMission();
            StopGameplay();
        }
    }
    void Awake()
    {
        if (aliveTanksText == null)
            aliveTanksText = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);
        EndingMenuPanel.SetActive(false);
    }
    void Update()
    {
        CheckEnemyTanks();
        aliveTanksText.text = "Active enemy tanks: " + aliveTanks + "/" + EnemyTanks.Length;
        if (PlayerTank == null || PlayerTank.IsDestroyed )//DEFEAT
        {
            DefeatPanel.SetActive(true);
            ShowPanel(DefeatPanel, defeatColour);
            MissionFinished = true;
            StopGameplay();
        }

        if(MissionFinished == true)
        {
            EndingMenuPanel.SetActive(true);
        }
    }
    private void ShowPanel(GameObject panel, Color specialColour)
    {
        panel.SetActive(true);
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeInPanel(canvasGroup, specialColour));    
    }

    private IEnumerator FadeInPanel(CanvasGroup canvasGroup, Color specialColour)
    {
        float timer = 0f;

        while (timer < panelAppearTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / panelAppearTime);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        StartCoroutine(ColourTransition(canvasGroup, specialColour));
    }
    private IEnumerator ColourTransition(CanvasGroup canvasGroup, Color specialColour)
    {
        while(true){
            float timer = 0f;

            while (timer < colourTransitionTime)
            {
                timer += Time.deltaTime;
                canvasGroup.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(normalColour, specialColour, timer / colourTransitionTime);
                yield return null;
            }

            timer = 0f;

            while (timer < colourTransitionTime)
            {
                timer += Time.deltaTime;
                canvasGroup.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(specialColour, normalColour, timer / colourTransitionTime);

                yield return null;
            }
        }
    }
    void StopGameplay()
    {
        canvas.gameObject.SetActive(false);

    }
}
