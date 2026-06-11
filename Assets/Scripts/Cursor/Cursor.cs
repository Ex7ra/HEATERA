using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControllerComplex : MonoBehaviour
{
    public static CursorControllerComplex Instance { get; private set; }

    [SerializeField] private Texture2D cursorTextureDefault;
    [SerializeField] private Texture2D cursorTextureTarget;
  
    [SerializeField] private GameObject PauseMenu;

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
        SetToMode(ModeOfCursor.Default);
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
