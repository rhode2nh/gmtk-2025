using System;
using UnityEngine;

public class HurtTest : MonoBehaviour
{
    [SerializeField] private float _damage;
    
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamagable>()?.TakeDamage(_damage);
    }
}
