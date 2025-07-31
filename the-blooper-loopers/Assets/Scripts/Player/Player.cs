using System;
using UnityEngine;
using Wannabuh.FPSController;

public class Player : FPSController, IDamagable, ICanPickup
{
    [field: Header("General Player Settings")]
    [field: SerializeField] public float Health { get; private set; }

    private FPSController _fpsController;

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public void Pickup(Item item)
    {
        // TODO: Add and apply item stats
        Debug.Log(item.name);
    }
}
