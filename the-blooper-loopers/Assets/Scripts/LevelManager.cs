using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [field: SerializeField] public int CurrentWave { get; private set; }
    [field: SerializeField] public int NumEnemiesToKill { get; private set; }

    private List<IResetHandler> _resetItems = new();
    private float _totalEnemiesKilled;
    private float _enemiesKilledForCurrentWave;

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
    }

    private void Start()
    {
        _resetItems = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IResetHandler>()
            .ToList();
    }

    public void ResetLevel()
    {
        foreach (var resetItem in _resetItems)
        {
            resetItem.Reset();
        }
    }

    public void IncrementEnemyKillCounter()
    {
        _enemiesKilledForCurrentWave++;
        _totalEnemiesKilled++;
        CheckWaveComplete();
    }

    private void CheckWaveComplete()
    {
        if (NumEnemiesToKill > _enemiesKilledForCurrentWave) return;
        
        NextWave();
    }

    private void NextWave()
    {
        ResetLevel();
        _enemiesKilledForCurrentWave = 0;
        CurrentWave++;
    }
}
