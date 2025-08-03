using System;
using System.Collections.Generic;
using UnityEngine;
using Wannabuh.FPSController;

public class Player : FPSController, IDamagable, ICanPickup, IStatNotifier
{
    [field: Header("General Player Settings")]
    [field: SerializeField] public float MaxHealth { get; private set; }

    public Action<float> OnUpdatePlayerUI;

    private float _currentHealth;
    
    // Stat Upgrade Values
    private float _additionalMaxHealth;
    private int _additionalJumps;

    protected override void Awake()
    {
        base.Awake();
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        _currentHealth -= damage;
        OnUpdatePlayerUI?.Invoke(_currentHealth / MaxHealth + _additionalMaxHealth);
        if (MaxHealth <= 0)
        {
            Debug.Log("Player is dead");
        }
    }

    public void Pickup(Item item)
    {
        Inventory.Instance.AddItem(item);
        _currentHealth = Mathf.Clamp(_currentHealth + item.Health, 0,  MaxHealth);
        OnUpdatePlayerUI?.Invoke(_currentHealth / MaxHealth);
    }

    public void UpdateStats(List<StatsData> statList)
    {
        _additionalJumps = 0;
        _additionalMaxHealth = 0;
        foreach (var statData in statList)
        {
            _additionalJumps += statData.Jumps;
            _additionalMaxHealth += statData.MaxHealth;
        }
        
        SetExtraJumps(_additionalJumps);
        
        OnUpdatePlayerUI?.Invoke(_currentHealth / MaxHealth + _additionalMaxHealth);
    }
}
