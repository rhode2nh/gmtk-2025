using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
        
    private List<IStatNotifier> _statNotifiers = new();
    private List<StatsData> _statList = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _statNotifiers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IStatNotifier>().ToList();
    }

    public void AddItem(Item item)
    {
        if (item.IsConsumable) return;
        
        _statList.Add(item.statsData);
        UpdateStats();
    }

    private void UpdateStats()
    {
        foreach (var statNotifier in _statNotifiers)
        {
            statNotifier.UpdateStats(_statList);
        }
    }
}
