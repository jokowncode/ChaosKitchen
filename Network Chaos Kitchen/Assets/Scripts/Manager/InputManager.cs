using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct InputSetting {
    public string MoveUp;
    public string MoveDown;
    public string MoveLeft;
    public string MoveRight;
    public string Interact;
    public string InteractAlt;
    public string Pause;
    public string Run;
}

public class InputManager : MonoBehaviour {

    public static InputManager Instance { get; private set; }

    public Action OnOneTimeInteract;
    public Action OnOneTimeInteractAlt;
    public Action<InputActionPhase> OnDurInteract;
    public Action OnPauseGame;
    public Action OnRun;
    
    private InputSystem_Actions InputActions;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        InputActions = new InputSystem_Actions();
        InputActions.Player.Enable();
        InputActions.Player.Interact.started += InteractOneTime;
        InputActions.Player.InteractAlt.started += InteractAltOneTime;
        
        InputActions.Player.InteractAlt.started += InteractDur;
        InputActions.Player.InteractAlt.performed += InteractDur;
        InputActions.Player.InteractAlt.canceled += InteractDur;
        
        InputActions.Player.Pause.started += PauseGame;
        InputActions.Player.Run.started += Run;
    }

    public void TogglePlayerInput(bool enable) {
        if (enable) {
            InputActions.Player.Enable();
        } else {
            InputActions.Player.Disable();
        }
    }

    public void RebindInput(InputType type, Action<InputActionRebindingExtensions.RebindingOperation> completeAction) {
        InputAction action;
        int bindingIndex;
        switch (type) {
            default:
            case InputType.MoveUp:
                action = InputActions.Player.Move;
                bindingIndex = 1;
                break;
            case InputType.MoveDown:
                action = InputActions.Player.Move;
                bindingIndex = 2;
                break;
            case InputType.MoveLeft:
                action = InputActions.Player.Move;
                bindingIndex = 3;
                break;
            case InputType.MoveRight:
                action = InputActions.Player.Move;
                bindingIndex = 4;
                break;
            case InputType.Interact:
                action = InputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case InputType.InteractAlt:
                action = InputActions.Player.InteractAlt;
                bindingIndex = 0;
                break;
            case InputType.Pause:
                action = InputActions.Player.Pause;
                bindingIndex = 0;
                break; 
            case InputType.Run:
                action = InputActions.Player.Run;
                bindingIndex = 0;
                break;
        }

        if (action == null) {
            throw new Exception($"Cant Support Rebind this InputType {type.ToString()}");
        }
        action.PerformInteractiveRebinding(bindingIndex).OnComplete(completeAction).Start();
    }

    public string GetRebindingJson() {
        return InputActions.SaveBindingOverridesAsJson();
    }

    public void UseRebindingJson(string json) {
        InputActions.LoadBindingOverridesFromJson(json);
    }

    public InputSetting GetCurrentInputSetting() {
        return new InputSetting() {
            MoveUp = InputActions.Player.Move.bindings[1].ToDisplayString(),
            MoveDown = InputActions.Player.Move.bindings[2].ToDisplayString(),
            MoveLeft = InputActions.Player.Move.bindings[3].ToDisplayString(),
            MoveRight = InputActions.Player.Move.bindings[4].ToDisplayString(),
            Interact = InputActions.Player.Interact.bindings[0].ToDisplayString(),
            InteractAlt = InputActions.Player.InteractAlt.bindings[0].ToDisplayString(),
            Pause = InputActions.Player.Pause.bindings[0].ToDisplayString(),
            Run = InputActions.Player.Run.bindings[0].ToDisplayString(),
        };
    }
    
    private void Run(InputAction.CallbackContext obj) {
        if(!GameManager.Instance.IsPlaying) return;
        OnRun?.Invoke();
    }
    
    private void PauseGame(InputAction.CallbackContext obj) {
        if(!GameManager.Instance.IsPlaying) return;
        OnPauseGame?.Invoke();
    }

    private void InteractOneTime(InputAction.CallbackContext obj) {
        if(!GameManager.Instance.IsPlaying) return;
        OnOneTimeInteract?.Invoke();
    }
    
    private void InteractAltOneTime(InputAction.CallbackContext obj) {
        if(!GameManager.Instance.IsPlaying) return;
        OnOneTimeInteractAlt?.Invoke();
    }

    private void InteractDur(InputAction.CallbackContext obj) {
        if(!GameManager.Instance.IsPlaying) return;
        OnDurInteract?.Invoke(obj.phase);
    }

    public Vector3 GetMoveVectorNormalized() {
        Vector2 move = InputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(move.x, 0.0f, move.y);
    }
}
