using UnityEngine;
using System.Collections;

public class SmokeEffect : MonoBehaviour
{
    [Header("Smoke Sprites")]
    public Sprite smallSmoke;
    public Sprite mediumSmoke;
    public Sprite largeSmoke;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float lifeTime = 0.45f;

    private SpriteRenderer sr;
    private Color startColor;

    private bool mediumShown = false;
    private bool largeShown = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = smallSmoke;

        startColor = sr.color;

        StartCoroutine(SmokeRoutine());
    }

    IEnumerator SmokeRoutine()
    {
        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / lifeTime;
            // Move away from the barrel
            float currentSpeed = Mathf.Lerp(moveSpeed, 0.2f, progress);
                transform.position += transform.up * currentSpeed * Time.deltaTime;

            

            // Change sprite only once
            if (!mediumShown && progress >= 0.33f)
            {
                sr.sprite = mediumSmoke;
                mediumShown = true;
            }

            if (!largeShown && progress >= 0.66f)
            {
                sr.sprite = largeSmoke;
                largeShown = true;
            }

            // Fade out
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, progress);
            sr.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}