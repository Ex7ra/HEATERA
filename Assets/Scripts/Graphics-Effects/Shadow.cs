using UnityEngine;

public class Shadow2D : MonoBehaviour
{
    public Transform target;                         // Hull or Tank root
    public Vector2 worldOffset = new Vector2(-0.06f, -0.09f);

    [Header("Size")]
    public Vector3 shadowLocalScale = new Vector3(0.53f, 0.58f, 1f);

    [Header("Rotation")]
    public bool rotateWithTarget = false;            // true = shadow rotates with tank
    public float extraRotationDegrees = 0f;          // if you want a slight twist

    void LateUpdate()
    {
        if (!target) return;

        transform.position = target.position + (Vector3)worldOffset;
        transform.localScale = shadowLocalScale;

        if (rotateWithTarget)
            transform.rotation = target.rotation * Quaternion.Euler(0f, 0f, extraRotationDegrees);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, extraRotationDegrees);
    }
}
