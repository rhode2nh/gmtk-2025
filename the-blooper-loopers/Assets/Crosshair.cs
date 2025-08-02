using System;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    private RawImage _rawImage;

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                Mathf.Infinity, _layerMask, QueryTriggerInteraction.Ignore))
        {
            _rawImage.color = hit.transform.GetComponent<TurretBehavior>() ? Color.red : Color.white;
        }
    }
}
