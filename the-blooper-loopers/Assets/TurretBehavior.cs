using Unity.VisualScripting;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed;
    public float rateOfFire;
    public float rotateThreshold;
    public GameObject turretProjectile;
    public bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger == true)
        {
            Rotate();
        }
    }

    void Shoot()
    {

    }

    void Rotate()
    {
        //turret start out oriented in a direction
        //when rotate is called, find the direction straight to the player
        //gradually move at rotation speed until the turret orientation == angle towards player

        var directionFromPlayerToTurret = (target.position - transform.position).normalized;
        float angle = Vector3.SignedAngle(transform.forward, directionFromPlayerToTurret, Vector3.up);
        
        if (Vector3.Dot(transform.forward, directionFromPlayerToTurret) > rotateThreshold)
        {
            transform.Rotate(new Vector3(0, angle * rotationSpeed * Time.deltaTime, 0));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInTrigger = true;
            target = other.GetComponent<Player>().transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInTrigger = false;
            target = null;
        } 
    }

}
