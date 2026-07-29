using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public float lifetime = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}