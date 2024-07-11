using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [Header("Keyboard Input Values")]
    public Vector2 move;
    public bool dodge;
    public bool ability;
    public bool fire;
    public bool reload;
    [Space]
    public bool menu;
    public bool inventory;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnDodge(InputValue value)
    {
        DodgeInput(value.isPressed);
    }

    public void OnAbility(InputValue value)
    {
        AbilityInput(value.isPressed);
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

    public void AbilityInput(bool newSprintState)
    {
        ability = newSprintState;
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
}
