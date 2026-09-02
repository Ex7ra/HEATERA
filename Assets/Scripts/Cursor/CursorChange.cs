using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ModeOfCursor modeOfCursor;

    [Header("Highlight")]
    [SerializeField] private SpriteRenderer[] highlightSprites;
    [SerializeField] private Color hoverColor = Color.yellow;

    private Color[] originalColors;
    private TankDamageReceiver tankDamageReceiver;
    private LevelIntro IntroManager;

    private void Awake()
    {
        tankDamageReceiver = GetComponentInParent<TankDamageReceiver>();
        originalColors = new Color[highlightSprites.Length];

        for (int i = 0; i < highlightSprites.Length; i++)
        {
            originalColors[i] = highlightSprites[i].color;
        }
    }
    void Start()
    {
        IntroManager = FindObjectOfType<LevelIntro>();
    }
    void Update()
    {
        if (IntroManager != null && !LevelIntro.gameplayStarted)
        {
            enabled = false;
            return;
        }
        else
        {
            enabled = true;
        }
            
        if (tankDamageReceiver.IsDestroyed)
        {
            for (int i = 0; i < highlightSprites.Length; i++)
            {
                highlightSprites[i].color = originalColors[i];
            }

            enabled = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorControllerComplex.Instance.SetToMode(modeOfCursor);

        foreach (var sr in highlightSprites)
        {
            sr.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorControllerComplex.Instance.SetToMode(ModeOfCursor.Default);

        for (int i = 0; i < highlightSprites.Length; i++)
        {
            highlightSprites[i].color = originalColors[i];
        }
    }
}