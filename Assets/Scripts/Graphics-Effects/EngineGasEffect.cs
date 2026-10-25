using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
public class EngineGasEffect : MonoBehaviour
{
    public float timeBeforeStartEngine = 5f;
    public int numberOfSmokes = 3;
    public float directionVariation = 10f;
    public float lifeTime = 1f;
    public GameObject smokePrefab;
    public EngineModule engine;
    [Header("Gas Sprites")]
    public Sprite smallGas;
    public Sprite mediumGas;
    public Sprite mediumLongGas;
    public Sprite largeGas;
    private Tank tank;
    private SpriteRenderer sr;
    private Color startColor;
    private Vector3 moveDirection;
    private bool isPlaying = false;
    void Start()
    {
        if(engine != null)
        {
            engine.OnDestroyedEvent += EngineAction;
            engine.OnRepairedEvent += EngineAction;
        }
        sr = GetComponent<SpriteRenderer>();
        tank = GetComponentInParent<Tank>();
        sr.sprite = smallGas;
        startColor = sr.color;
        float angle = Random.Range(-directionVariation, directionVariation);
        moveDirection = Quaternion.Euler(0, 0, angle) * transform.up;

    }

    
    void Update()
    {
        
        if(tank.isAccelerating == true && !isPlaying)
        {
            isPlaying = true;
            StartCoroutine(EngineGas());
        }
    }
    
    void EngineAction()
    {
        if (engine.IsDestroyed)
        {
            
        }
        else
        {
            StartCoroutine(TimeR());
        }
    }
    IEnumerator EngineGas()
    {
        while(tank.isAccelerating)
        {
            for(int i = 0; i < numberOfSmokes; i++){
            GameObject smoke = Instantiate(smokePrefab, transform.position, transform.rotation);
           
            }
            yield return new WaitForSeconds(Random.Range(0.15f, 0.45f));
        }
        isPlaying = false;  
    }
    IEnumerator TimeR()
    {
        float timeR = 0f;
        while(timeR < timeBeforeStartEngine)
        {
            timeR += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(EngineGasStartUp());
    }
    IEnumerator EngineGasStartUp()
    {
        float timer = 0f;
        while(timer < lifeTime)
        {
            timer += Time.deltaTime;
            for(int i = 0; i < numberOfSmokes; i++)
            {
                GameObject smoke = Instantiate(smokePrefab, transform.position, transform.rotation);
            }
            yield return null;
        }
    }
}
