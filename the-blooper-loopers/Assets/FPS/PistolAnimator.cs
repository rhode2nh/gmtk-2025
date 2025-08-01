using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PistolAnimator : MonoBehaviour
{
    [SerializeField] private float _swaySpeed;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _moveTransitionSpeed;
    [SerializeField] private float _moveSpeed;
    
    private Animator _animator;
    private FPSActions _fpsActions;
    private Vector2 _look;
    private Vector2 _currentSway;
    private Vector2 _swayVelocity;
    private Weapon _weapon;

    private float _move;
    private float _lerpedMove;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _fpsActions = InputManager.Instance.FPSActions;
        _weapon = GetComponent<Weapon>();
        _weapon.OnShoot += OnShoot;
    }

    private void OnEnable()
    {
        _fpsActions.Player.Look.performed += GetLook;
        _fpsActions.Player.Move.performed += GetMove;
        _fpsActions.Player.Look.Enable();
        _fpsActions.Player.Move.Enable();
    }

    private void OnDisable()
    {
        _fpsActions.Player.Look.performed -= GetLook;
        _fpsActions.Player.Move.performed -= GetMove;
        _fpsActions.Player.Look.Disable();
        _fpsActions.Player.Move.Disable();
    }

    private void Update()
    {
        Sway();
        Move();
    }

    private void Sway()
    {
        Vector2 swayInput = Vector2.ClampMagnitude(_look * _sensitivity, 1);

        _currentSway = Vector2.Lerp(_currentSway, swayInput, Time.deltaTime * _swaySpeed);
        
        _animator.SetFloat("x", -_currentSway.x);
        _animator.SetFloat("y", _currentSway.y);
    }

    private void Move()
    {
        _lerpedMove = Mathf.Lerp(_lerpedMove, _move, Time.deltaTime * _moveTransitionSpeed);
        _animator.SetLayerWeight(2, _lerpedMove);
        _animator.SetFloat("MoveSpeed", _moveSpeed);
    }

    private void GetLook(InputAction.CallbackContext ctx)
    {
        _look = ctx.ReadValue<Vector2>();
    }

    private void GetMove(InputAction.CallbackContext ctx)
    {
        _move = InputUtils.IsPressed(ctx.ReadValue<Vector2>()) ? 1.0f : 0.0f;
    }

    private void OnShoot()
    {
        _animator.Play("Shoot", 0, 0f);
    }
}
