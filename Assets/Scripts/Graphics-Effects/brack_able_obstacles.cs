using UnityEngine;

public class brack_able_obstacles : MonoBehaviour
{
    public int health = 20;
    public SpriteRenderer normalSprite;
    public SpriteRenderer destroyedSprite;
    public CircleCollider2D obstacleCollider;
    private bool destroyed = false;
    void Start()
    {
        if (destroyedSprite != null)
            destroyedSprite.enabled = false;
    }

    
    public void TakeDamage(int damage)
    {
        if (destroyed)
            return;

        health -= damage;

        Debug.Log("Obstacle damaged! HP: " + health);

        if (health <= 0)
        {
            Destroyed();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (destroyed)
            return;

        TankController_ModernClutch tank = 
            collision.gameObject.GetComponent<TankController_ModernClutch>();

        if (tank != null)
        {
            float speed = collision.relativeVelocity.magnitude;

            Debug.Log("Tank hit obstacle at speed: " + speed);

            int collisionDamage = Mathf.RoundToInt(speed * 5f);

            TakeDamage(collisionDamage);
        }
    }


    void Destroyed()
    {
        destroyed = true;

        
        if (obstacleCollider != null)// Stop being obstacle
            obstacleCollider.enabled = false;

        
        if (normalSprite != null)// hide normal Sprite
            normalSprite.enabled = false;

        
        if (destroyedSprite != null)//show destroyed sprite
            destroyedSprite.enabled = true;

        Debug.Log("Obstacle destroyed!");
    }
}
