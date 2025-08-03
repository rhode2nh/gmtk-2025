using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour, IStatNotifier
{
    [SerializeField] private float _fireRate;
    [SerializeField] private float _damage;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Camera _fpsCamera;

    public event Action OnShoot;

    private FPSActions _fpsActions;
    private bool _isShooting;
    private bool _coroutineStarted;
    
    // Stat Upgrade Values
    private float _damageMultiplier = 1.0f;
    private float _fireRateMultiplier = 1.0f;

    AudioSource audioWeaponFire;

    private void Awake()
    {
        _fpsActions = InputManager.Instance.FPSActions;
    }

    private void Start()
    {
        var baseCamData = Camera.main.GetUniversalAdditionalCameraData();
        var fpsCameraData = _fpsCamera.GetUniversalAdditionalCameraData();

        audioWeaponFire = GetComponent<AudioSource>();

        if (!baseCamData.cameraStack.Contains(_fpsCamera))
        {
            baseCamData.cameraStack.Add(_fpsCamera);
        }
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
                    damagable.TakeDamage(_damage * _damageMultiplier);
                }
            }
            OnShoot?.Invoke();
            audioWeaponFire.Play();
            yield return new WaitForSeconds(_fireRate / _fireRateMultiplier);
        }

        _coroutineStarted = false;
    }

    public void UpdateStats(List<StatsData> statList)
    {
        _damageMultiplier = 1f;
        _fireRateMultiplier = 1f;
        foreach (var statData in statList)
        {
            _damageMultiplier += statData.Damage;
            _fireRateMultiplier += statData.FireRateMultiplier;
        }
        
        Debug.Log(_fireRateMultiplier);
    }
}