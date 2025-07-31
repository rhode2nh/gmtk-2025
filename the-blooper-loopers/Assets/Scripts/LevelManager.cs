using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private List<IResetHandler> _resetItems = new();

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
}
