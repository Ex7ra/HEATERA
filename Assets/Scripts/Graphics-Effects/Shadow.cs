using UnityEngine;

public class Shadow2D : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset = new Vector3(-0.06f, -0.09f, 0f);

    [Header("Size")]
    public Vector3 shadowLocalScale = new Vector3(0.53f, 0.58f, 1f);

    [Header("Rotation")]
    public bool rotateWithTarget = false;
    public float extraRotationDegrees = 0f;

    [Header("Sprites")]
    public SpriteRenderer normalSprite;
    public Sprite changedSprite;

    private TankDamageReceiver tankDamageReceiver;
    private bool changed;

    void Awake()
    {
        tankDamageReceiver = GetComponentInParent<TankDamageReceiver>();

        transform.localScale = shadowLocalScale;
    }

    void LateUpdate()
    {
        if (!target)
            return;

        transform.position = target.position + worldOffset;
        if (rotateWithTarget)
        {
            transform.rotation = target.rotation * Quaternion.Euler(0f, 0f, extraRotationDegrees);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, extraRotationDegrees);
        }

        if (!changed && tankDamageReceiver != null && tankDamageReceiver.IsDestroyed)
        {
            normalSprite.sprite = changedSprite;
            changed = true;
        }
    }
}