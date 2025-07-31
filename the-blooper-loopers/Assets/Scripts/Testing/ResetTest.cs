using System;
using UnityEngine;

public class ResetTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Player>()) return;
        
        LevelManager.Instance.ResetLevel();
    }
}
