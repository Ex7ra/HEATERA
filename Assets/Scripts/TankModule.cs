using UnityEngine;

public class TankModule : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    void Awake()
    {
        health = maxHealth;
    }

    public virtual void Damage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            OnDestroyed();
    }

    protected virtual void OnDestroyed()
    {
        Debug.Log(name + " destroyed");
    }
}
