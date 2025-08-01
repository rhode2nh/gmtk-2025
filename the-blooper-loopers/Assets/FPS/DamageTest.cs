using UnityEngine;

public class DamageTest : MonoBehaviour, IDamagable
{
    [SerializeField] private float _health;
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
