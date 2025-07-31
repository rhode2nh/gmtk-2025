using System;
using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour, IResetHandler
{
    [SerializeField] private GameObject _objectToSpawn;
    [SerializeField] private bool _respawn;
    [SerializeField] private float _respawnTime;

    public void Start()
    {
        Spawn();
    }

    public void Reset()
    {
        Debug.Log("Resetting");
    }

    private void Spawn()
    {
        var instantiatedObject = Instantiate(_objectToSpawn, transform);
        if (!instantiatedObject.TryGetComponent(out ILifecycleNotifier notifier)) return;
        
        notifier.OnDone += StartSpawnTimer;
    }

    private void StartSpawnTimer()
    {
        if (!_respawn) return;

        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(_respawnTime);
        Spawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
