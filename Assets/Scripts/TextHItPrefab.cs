using UnityEngine;

public class TextHitPrefab : MonoBehaviour
{
    public float speed = 1.5f;

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
