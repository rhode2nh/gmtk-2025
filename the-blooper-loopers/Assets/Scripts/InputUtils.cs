using UnityEngine;
using UnityEngine.InputSystem;

public static class InputUtils
{
    public static bool IsPressed(InputAction.CallbackContext ctx)
    {
        return ctx.ReadValue<float>() != 0.0f;
    }

    public static bool IsPressed(Vector2 input)
    {
        return input.x != 0.0f || input.y != 0.0f;
    }
}
