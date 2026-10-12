using UnityEngine;
using System;

public class TankModule : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public event Action OnDestroyedEvent;
    public event Action OnRepairedEvent;

    public bool IsDestroyed { get; private set; }

    protected virtual void Awake()
    {
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

            OnDestroyed();
            OnDestroyedEvent?.Invoke();
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