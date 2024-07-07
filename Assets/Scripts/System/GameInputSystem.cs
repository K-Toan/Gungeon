using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    [Header("Keyboard Input Values")]
    public bool menu;

    [Header("Settings")]
    public bool cursorLocked = false;

    public void OnMenu(InputValue value)
    {
        MenuInput(value.isPressed);
    }

    public void MenuInput(bool newState)
    {
        menu = newState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
