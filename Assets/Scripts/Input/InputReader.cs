using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scripts/Input/InputReader")]

//I barely understand why this class is needed
public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate{ };
    public event UnityAction<Vector2, bool> Look = delegate{ }; //boolean is if player is using mouse 
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };
    public event UnityAction<bool> Run = delegate { };
    public event UnityAction<bool> Dash = delegate { };
    public event UnityAction<bool> Attack = delegate { };

    PlayerInputActions inputActions;

    public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();

    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(instance: this);
        }
        inputActions.Enable();
    }

    public void EnablePlayerActions()
    {
        inputActions.Enable();
    }

    public void DisablePlayerActions()
    {
        inputActions.Disable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(arg0: context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    //simple inline method to check if input device is a mouse
    bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Attack.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Attack.Invoke(false);
                break;
        } 
    }

    //when input action phase is started, invoke enablemousecontrol event, otherwise disable
    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera.Invoke();
                break;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //no op
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Run.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Run.Invoke(false);
                break;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Dash.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Dash.Invoke(false);
                break;
        } 
    }
}
