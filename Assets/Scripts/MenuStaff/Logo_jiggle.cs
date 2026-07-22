using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

public class Logo_jiggle : MonoBehaviour, IPointerClickHandler
{
    public float angle = 20f;
    public float duration = 0.15f;
    public float finalSize = 1.2f;
    private Quaternion normalRotation;
    private Vector2 normalSize;
    void Start()
    {
        normalRotation = transform.rotation;
        normalSize = transform.localScale = new Vector2(0.35f, 0.35f);
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(Jiggle());
    }
    
    IEnumerator Jiggle()
    {
        Quaternion targetRotation = normalRotation * Quaternion.Euler(0, 0, angle);
        Vector3 targetSize = normalSize * finalSize;
        float t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(normalRotation, targetRotation, t/duration);
            transform.localScale = Vector3.Lerp(normalSize, targetSize, t / duration);
            yield return null;
        }
        
        t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(targetRotation, normalRotation, t/duration);
            transform.localScale = Vector3.Lerp(targetSize, normalSize, t / duration);
            yield return null;
        }
        transform.rotation = normalRotation;
        transform.localScale = normalSize;

        
        
    }

    

    
}
