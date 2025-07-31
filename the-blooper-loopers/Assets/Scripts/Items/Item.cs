using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<ICanPickup>()?.Pickup(this);
        OnPickedUp();
    }

    private void OnPickedUp()
    {
        Destroy(gameObject);
    }
}
