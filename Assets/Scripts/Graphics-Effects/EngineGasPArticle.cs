using UnityEngine;

public class EngineGasParticle : MonoBehaviour
{
    public Sprite[] gasSprites;
    public float lifeTime = 1f;
    public float minSpeed = 0.5f;
    public float maxSpeed = 2f;
    private SpriteRenderer sr;
    private Color startColor;
    private float timer = 0f;
    private Vector3 moveDirection;
    private float moveSpeed;
    public enum GasDirection
    {
        Down,
        Left,
        Right
    }
    public GasDirection direction;
    void Start()
    {
       
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        int randomIndex = Random.Range(0, gasSprites.Length);
        sr.sprite = gasSprites[randomIndex];
        float angle = Random.Range(-20f, 20f);
        Vector3 baseDirection = Vector3.down;

        switch (direction)
        {
            case GasDirection.Down:
                baseDirection = transform.up * -1f;
                break;

            case GasDirection.Left:
                baseDirection = -transform.right;
                break;

            case GasDirection.Right:
                baseDirection = transform.right;
                break;
        }

        moveDirection = Quaternion.Euler(0f, 0f, angle) * baseDirection;
        Debug.Log(angle);
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float progress = timer / lifeTime;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, progress);
        sr.color = c;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}