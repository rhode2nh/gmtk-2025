using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _cameraPitchTarget;
    public float smoothTime = 0.08f;

    private Vector3 velocity = Vector3.zero;
    private float _cameraPitch;
    private float _sensitivity = 10f;
    private Vector2 _lookInput;
    private FPSActions _fpsActions;

    private void Awake()
    {
        _fpsActions = new FPSActions();
    }

    private void OnEnable()
    {
        _fpsActions.Player.Look.performed += OnLook;
        _fpsActions.Player.Look.Enable();
    }

    private void OnDisable()
    {
        _fpsActions.Player.Look.Disable();
    }

    void LateUpdate()
    {
        if (_target == null) return;

        Vector3 targetPosition = _cameraPitchTarget.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            _cameraPitch -= _lookInput.y * _sensitivity * Time.deltaTime;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -90, 90);

            transform.localRotation = Quaternion.Euler(_cameraPitch, _target.eulerAngles.y, 0);
    }
    
    private void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }
}
