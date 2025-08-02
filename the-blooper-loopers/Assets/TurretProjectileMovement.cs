using Unity.Cinemachine;
using UnityEngine;

public class TurretProjectileMovement : MonoBehaviour
{
    public float velocity;
    public float timeAlive;
    public float damage;
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, timeAlive);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * velocity * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        var player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(this.gameObject);
        
    }

}