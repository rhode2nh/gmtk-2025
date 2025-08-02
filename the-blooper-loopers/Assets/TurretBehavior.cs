using System;
using UnityEngine;

public class TurretBehavior : MonoBehaviour, IDamagable, ILifecycleNotifier
{
    public Transform target;
    public float rotationSpeed;
    public float rateOfFire;
    public float timeElapsed;
    public float rotateThreshold;
    public GameObject turretProjectile;
    public Transform bulletSpawner;
    public bool playerInTrigger;
    public float health;

    public event Action OnDone;

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

        timeElapsed += Time.deltaTime;

    }

    void Shoot()
    {
        Instantiate(turretProjectile, bulletSpawner.position, bulletSpawner.rotation);
        
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

        if (timeElapsed > rateOfFire)
        {
            Shoot();
            timeElapsed = 0;
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDone?.Invoke();
        OnDone = null;
        LevelManager.Instance.IncrementEnemyKillCounter();
        Destroy(gameObject);
    }
}
