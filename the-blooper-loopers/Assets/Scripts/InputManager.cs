using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public FPSActions FPSActions { get; private set; }

    private void Awake()
    {
        Instance = this;
        FPSActions = new FPSActions();
    }
}
