using UnityEngine;

public class TankModule : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public bool IsDestroyed { get; protected set; }  

    protected virtual void Awake()
    {
        health = maxHealth;
        IsDestroyed = false;
    }

    public virtual void Damage(float dmg)
    {
        if (IsDestroyed) return;

        health -= dmg;

        if (health <= 0f)
        {
            health = 0f;
            IsDestroyed = true;
            OnDestroyed();
        }
    }

    protected virtual void OnDestroyed()
    {
        Debug.Log(name + " destroyed");
    }
}