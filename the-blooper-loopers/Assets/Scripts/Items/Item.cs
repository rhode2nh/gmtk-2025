using System;
using UnityEngine;

public class Item : MonoBehaviour, ILifecycleNotifier
{
    [field: SerializeField] public bool IsConsumable { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    [SerializeField] private float _rotationSpeed;
    [field: SerializeField] public StatsData statsData { get; private set; }
    
    public event Action OnDone;

    private void Update()
    {
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<ICanPickup>()?.Pickup(this);
        OnPickedUp();
    }

    private void OnPickedUp()
    {
        OnDone?.Invoke();
        OnDone = null;
        Destroy(gameObject);
    }
}
