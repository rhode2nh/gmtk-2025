using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player _player;

    private RawImage _healthBar;

    private void Awake()
    {
        _healthBar = GetComponent<RawImage>();
    }

    private void OnEnable()
    {
        _player.OnUpdatePlayerUI += UpdateUI;
    }

    private void OnDisable()
    {
        _player.OnUpdatePlayerUI -= UpdateUI;
    }
    
    private void UpdateUI(float healthPercentage)
    {
        _healthBar.rectTransform.localScale = new Vector3(healthPercentage, 1.0f, 1.0f);
    }
}
