using System;
using UnityEngine;

public class Item : MonoBehaviour, ILifecycleNotifier
{
    [field: SerializeField] public float Health { get; private set; }
    
    public event Action OnDone;
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
