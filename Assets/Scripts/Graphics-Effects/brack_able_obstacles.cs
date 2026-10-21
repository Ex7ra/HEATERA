using UnityEngine;

public class brack_able_obstacles : MonoBehaviour
{
    public bool softObstacle = false; //If true, shell which hits this obstacle will not be destroyed, but will continue its path. If false, shell will be destroyed on impact
    public int health = 20;
    public SpriteRenderer normalSprite;
    public SpriteRenderer destroyedSprite;
    public Collider2D obstacleCollider;
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

        //Debug.Log("Damage received: " + damage);
        //Debug.Log("Health before damage: " + health);
        health -= damage;

        //Debug.Log("Health after damage: " + health);

        if (health <= 0)
        {
            Destroyed();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (destroyed)
            return;

        Tank tank = collision.gameObject.GetComponent<Tank>();

        if (tank != null)
        {
            float speed = collision.relativeVelocity.magnitude;
            float turnSpeed = tank.CurrentTurn;
            //Debug.Log("Tank hit obstacle at speed: " + speed + ", Turn Speed: " + turnSpeed);
            int collisionDamage = Mathf.RoundToInt((turnSpeed + 5f) * speed * 5f);

            // If this is a soft obstacle, tell the tank to keep its momentum
            if (softObstacle)
            {
                TankController_ModernClutch tankController = collision.gameObject.GetComponent<TankController_ModernClutch>();

                if (tankController != null)
                {
                    tankController.KeepMomentum();
                }
            }
            int damage = Mathf.Max(collisionDamage, 2); 
            TakeDamage(damage);
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null && rb.linearVelocity.magnitude > 0f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
       
        Debug.Log("Obstacle destroyed!");
    }
}
