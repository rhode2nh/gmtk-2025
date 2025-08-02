using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WormBehaviour : MonoBehaviour
{

    public Transform target;
    public float movespeed;
    public float attackspeed;
    private bool isintrigger;
    public NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isintrigger)
        {
            
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            target = other.GetComponent<Transform>();
            Debug.Log(target.position.ToString());
            agent.SetDestination(target.position);
            isintrigger = true;
        }
    }
    void OnTriggerExit(Collider Other)
    {
        if (Other.GetComponent<Player>() != null)
        {
            isintrigger = false;
        }
    }
}
