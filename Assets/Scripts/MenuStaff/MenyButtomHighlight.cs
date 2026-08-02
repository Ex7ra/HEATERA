using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject arrow;

    private Coroutine blinkCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        blinkCoroutine = StartCoroutine(BlinkArrow());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        arrow.SetActive(false);
    }

    IEnumerator BlinkArrow()
    {
        while (true)
        {
            arrow.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            arrow.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}