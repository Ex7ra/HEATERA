using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
public class EngineGasEffect : MonoBehaviour
{
    public float directionVariation = 10f;
    public float lifeTime = 1f;
    public GameObject smokePrefab;
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
    
    IEnumerator EngineGas()
    {
        float timer = 0f;
        while(timer < lifeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / lifeTime;
            Instantiate(smokePrefab, transform.position, transform.rotation);
            


            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, progress);
            sr.color = c;

            yield return new WaitForSeconds(Random.Range(0.15f, 0.45f));
        }
        Destroy(smokePrefab);
    }
}
