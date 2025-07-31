using System;
using UnityEngine;
using Wannabuh.FPSController;

public class Player : FPSController, IDamagable, ICanPickup
{
    [field: Header("General Player Settings")]
    [field: SerializeField] public float MaxHealth { get; private set; }

    public Action<float> OnUpdatePlayerUI;

    private float _currentHealth;

    protected override void Awake()
    {
        base.Awake();
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        _currentHealth -= damage;
        OnUpdatePlayerUI?.Invoke(_currentHealth / MaxHealth);
        if (MaxHealth <= 0)
        {
            Debug.Log("Player is dead");
        }
    }

    public void Pickup(Item item)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + item.Health, 0,  MaxHealth);
        Debug.Log(_currentHealth);
        OnUpdatePlayerUI?.Invoke(_currentHealth / MaxHealth);
    }
}
