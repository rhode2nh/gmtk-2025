using System;
using UnityEngine;
using UnityEngine.AI;
using Wannabuh.FPSController;

public class NewBehaviourScript : MonoBehaviour
{
    // [SerializeField] private Transform _destination;
    [SerializeField] private float _updatePathThreshold;
    
    private NavMeshAgent _agent;
    
    private float _elapsedTime = 0.0f;
    private FPSController _fpsController;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // _agent.SetDestination(_destination.position);
    }

    private void Update()
    {
        if (_elapsedTime >= _updatePathThreshold)
        {
            if (_fpsController != null)
            {
                _agent.SetDestination(_fpsController.transform.position);
                // _agent.SetDestination(_destination.position);
                Debug.Log("Path Updated");
            }
            
            _elapsedTime = 0.0f;
        }
        else
        {
            _elapsedTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out FPSController fpsController)) return;

        Debug.Log("FPS controller is in trigger");
        _fpsController = fpsController;
        _agent.SetDestination(_fpsController.transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<FPSController>()) return;

        Debug.Log("FPS controller is out of trigger");
        _fpsController = null;
    }
}
