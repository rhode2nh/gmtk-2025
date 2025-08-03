using System;

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WormBehaviour : MonoBehaviour, IDamagable, ICrosshairNotifier, ILifecycleNotifier
{
    public NavMeshAgent agent;
    public Collider sightCollider;
    public float attackSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackDelay;
    public float health;
    public float speed;
    public float aggroDuration;
    public float chaseInterval;
    public float followDistance;
    private bool hasTarget;
    private float updateTargetTimer;
    private float aggroTimer;
    private float attackTimer;
    private bool isInTrigger;
    private Player player;
    private Vector3 playerPos;

    public event Action OnDone;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            float time = Time.deltaTime;
            updateTargetTimer += time;
            attackTimer += time;
            if (!isInTrigger)
            {
                aggroTimer -= time;
            }
            if (updateTargetTimer >= chaseInterval)
            {
                UpdateTarget();
            }
            if (aggroTimer <= 0)
            {
                hasTarget = false;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            aggroTimer = aggroDuration;
            player = other.GetComponent<Player>();
            UpdateTarget();
            isInTrigger = true;
            hasTarget = true;
        }
    }
    void OnTriggerExit(Collider Other)
    {

        if (Other.GetComponent<Player>() != null)
        {

            isInTrigger = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (isInTrigger)
        {
            if (RangeCheck(attackRange))
            {
                ///if it is time to attack, stop the agent and then delay before attacking
                if (attackTimer >= attackSpeed)
                {
                    attackTimer = 0;
                    StartCoroutine(Attack());
                }
            }
        }
    }
    void UpdateTarget()
    {
        {
            if (!RangeCheck(followDistance))
            {
                updateTargetTimer = 0;
                playerPos = player.GetComponent<Transform>().position;
                agent.SetDestination(playerPos);
                updateTargetTimer = 0;
            }
            else
            {
                agent.ResetPath();
                updateTargetTimer = 0;
            }
        }
    }
    IEnumerator Attack()
    {
        agent.speed = 0;
        Debug.Log("Attack!");
        yield return new WaitForSeconds(attackDelay);
        if (RangeCheck(attackRange))
        {
            player.TakeDamage(attackDamage);
        }
        agent.speed = speed;
    }
    bool RangeCheck(float distance)
    {
        playerPos = player.GetComponent<Transform>().position;
        Vector3 wormPosition = GetComponent<Transform>().position;
        return Vector3.Distance(wormPosition, playerPos) <= distance;
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
