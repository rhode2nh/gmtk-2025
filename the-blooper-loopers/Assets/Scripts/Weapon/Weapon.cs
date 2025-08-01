using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _fireRate;
    [SerializeField] private float _damage;
    [SerializeField] private LayerMask _layerMask;

    public event Action OnShoot;

    private FPSActions _fpsActions;
    private bool _isShooting;
    private bool _coroutineStarted;

    private void Awake()
    {
        _fpsActions = InputManager.Instance.FPSActions;
    }

    private void OnEnable()
    {
        _fpsActions.Player.Shoot.performed += Shoot;
        _fpsActions.Player.Shoot.Enable();
    }

    private void OnDisable()
    {
        _fpsActions.Player.Shoot.performed += Shoot;
        _fpsActions.Player.Shoot.Disable();
    }

    private void Update()
    {
        if (!_isShooting || _coroutineStarted) return;
        
        StartCoroutine(ShootLoop());
    }

    private void Shoot(InputAction.CallbackContext ctx)
    {
        _isShooting = InputUtils.IsPressed(ctx);
    }

    private IEnumerator ShootLoop()
    {
        _coroutineStarted = true;
        while (_isShooting)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                    Mathf.Infinity, _layerMask,
                    QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(_damage);
                }
            }
            OnShoot?.Invoke();
            yield return new WaitForSeconds(_fireRate);
        }

        _coroutineStarted = false;
    }
}