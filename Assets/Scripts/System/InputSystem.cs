using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    [Header("Keyboard Input Values")]
    public Vector2 move;
    public bool dodge;
    public bool dash;
    public bool fire;
    public bool reload;
    [Space]
    public bool menu;
    public bool inventory;

    [Header("Settings")]
    public bool cursorLocked = false;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnDodge(InputValue value)
    {
        DodgeInput(value.isPressed);
    }

    public void OnDash(InputValue value)
    {
        DashInput(value.isPressed);
    }

    public void OnFire(InputValue value)
    {
        FireInput(value.isPressed);
    }

    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }

    public void OpenMenu(InputValue value)
    {
        MenuInput(value.isPressed);
    }

    public void OpenInventory(InputValue value)
    {
        InventoryInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void DashInput(bool newSprintState)
    {
        dash = newSprintState;
    }

    public void DodgeInput(bool newSprintState)
    {
        dodge = newSprintState;
    }

    public void FireInput(bool newSprintState)
    {
        fire = newSprintState;
    }

    public void ReloadInput(bool newSprintState)
    {
        reload = newSprintState;
    }

    public void MenuInput(bool newSprintState)
    {
        menu = newSprintState;
    }

    public void InventoryInput(bool newSprintState)
    {
        inventory = newSprintState;
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
