using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControllerComplex : MonoBehaviour
{
    public static CursorControllerComplex Instance { get; private set; }

    [SerializeField] private Texture2D cursorTextureDefault;
    [SerializeField] private Texture2D cursorTextureTarget;
  
    [SerializeField] private GameObject PauseMenu;
    
    private LevelIntro IntroManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        IntroManager = FindObjectOfType<LevelIntro>();
        
        if (IntroManager != null && !LevelIntro.gameplayStarted)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            SetToMode(ModeOfCursor.Default);
        }
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (IntroManager == null)
        {
            IntroManager = FindObjectOfType<LevelIntro>();
        }

        if (IntroManager != null && !LevelIntro.gameplayStarted)
        {
            Cursor.visible = false;
            return;
        }

        if (IntroManager != null && LevelIntro.gameplayStarted)
        {
            Cursor.visible = true;
            SetToMode(ModeOfCursor.Default);
        }
        
    }
    
    public void SetToMode(ModeOfCursor modeOfCursor)
    {
        Texture2D tex = cursorTextureDefault;
        Vector2 hotSpot = new Vector2(tex.width / 2f, tex.height / 2f); // Center for default

        if (modeOfCursor == ModeOfCursor.Target)
        {
            tex = cursorTextureTarget;
            // Automatically center the hotspot for the target/crosshair
            hotSpot = new Vector2(tex.width / 2f, tex.height / 2f);
        }

        Cursor.SetCursor(tex, hotSpot, CursorMode.Auto);
    }
}

public enum ModeOfCursor { Default, Target }
