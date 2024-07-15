using UnityEngine;
using UnityEngine.InputSystem;

public class GunInputSystem : MonoBehaviour
{
    [Header("Keyboard Input Values")]
    public bool fire;
    public bool reload;

    public void OnFire(InputValue value)
    {
        FireInput(value.isPressed);
    }

    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }

    public void FireInput(bool newSprintState)
    {
        fire = newSprintState;
    }

    public void ReloadInput(bool newSprintState)
    {
        reload = newSprintState;
    }

    private void OnEnable()
    {
        fire = false;
        reload = false;
    }

    private void OnDisable()
    {
        fire = false;
        reload = false;
    }
}
