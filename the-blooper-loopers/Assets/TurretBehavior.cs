using System;
using Unity.Mathematics;
using UnityEngine;

public class TurretBehavior : MonoBehaviour, IDamagable, ILifecycleNotifier, ICrosshairNotifier
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

    AudioSource audioWeaponFire;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioWeaponFire = GetComponent<AudioSource>();
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
        audioWeaponFire.Play();
    }

    void Rotate()
    {
        //turret start out oriented in a direction
        //when rotate is called, find the direction straight to the player
        //gradually move at rotation speed until the turret orientation == angle towards player

        var directionFromPlayerToTurret = (target.position - transform.position).normalized;
        var targetRotation = Quaternion.LookRotation(directionFromPlayerToTurret);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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
