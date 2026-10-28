using UnityEngine;
using System;

public class TankModule : MonoBehaviour
{
    private ModuleDamageVisual damageVisual;
    public float maxHealth = 100f;
    public float health;

    public event Action OnDestroyedEvent;
    public event Action OnRepairedEvent;

    public bool IsDestroyed { get; private set; }

    protected virtual void Awake()
    {
        damageVisual = GetComponent<ModuleDamageVisual>();
        health = maxHealth;
        IsDestroyed = false;
    }

    
    public virtual void Damage(float dmg)
    {
        if (IsDestroyed)
            return;

        health -= dmg;

        if (health <= 0f)
        {
            health = 0f;
            IsDestroyed = true;

            damageVisual?.ShowDamage(true);
            OnDestroyed();
            OnDestroyedEvent?.Invoke();
        }
        else
        {
            damageVisual?.ShowDamage(false);
        }
    }


    public virtual void Repair()
    {
        health = maxHealth;
        IsDestroyed = false;

        OnRepairedEvent?.Invoke();

        Debug.Log(name + " repaired");
    }

    protected virtual void OnDestroyed()
    {
        Debug.Log(name + " destroyed");
    }
}