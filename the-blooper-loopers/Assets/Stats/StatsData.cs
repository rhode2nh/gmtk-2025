using UnityEngine;

[CreateAssetMenu(fileName = "StatsData", menuName = "Stats/New Stats")]
public class StatsData : ScriptableObject
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float FireRateMultiplier { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public int Jumps { get; private set; }
}
