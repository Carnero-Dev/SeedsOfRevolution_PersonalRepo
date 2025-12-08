using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputListener", menuName = "SOR/Input/InputListener")]
public class InputListener : ScriptableObject, PlayerInput.IPlayerActions, PlayerInput.IUIMenuActions {

    public enum GameModes {
        Game, UI
    }

    #region EVENTS

    public Action<Vector2> OnMoveEvent;
    public Action OnLeftStartClickEvent;
    public Action OnLeftCancelClickEvent;
    public Action OnRightStartedClickEvent; 
    public Action OnRightCanceledClickEvent; 
    public Action OnShowStatsEvent;
    public Action OnShowEnemyStatsEvent;
    public Action OnShowActionsWindowEvent;
    public Action OnShowNewsWindowEvent;
    public Action<Vector2> OnZoomEvent;
    public Action OnPauseEvent;
    public Action OnResumeEvent;
    public Action OnStopResumeTimeEvent;
    public Action OnIncrementTimeEvent;
    public Action OnDecrementTimeEvent;
    public Action<int> OnSetTimeScale;

    #endregion

    private PlayerInput _playerInput;

    private void OnEnable() {
        if (_playerInput == null) {
            _playerInput = new PlayerInput();

            _playerInput.Player.SetCallbacks(this);
            _playerInput.UIMenu.SetCallbacks(this);

            ChangeGameMode(GameModes.Game);
        }
    }

    void OnDisable() {
        _playerInput.Player.Disable();
    }

    public void ChangeGameMode(GameModes gMod) {
        switch (gMod) {
            case GameModes.Game:
                _playerInput.Player.Enable();
                _playerInput.UIMenu.Disable();
                break;

            case GameModes.UI:
                _playerInput.Player.Disable();
                _playerInput.UIMenu.Enable();
                break;
        }
    }

    #region IPlayerActions Implementation
    void PlayerInput.IPlayerActions.OnMove(InputAction.CallbackContext context) {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void PlayerInput.IPlayerActions.OnLeftClick(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed){
            OnLeftStartClickEvent?.Invoke();
        }

        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled) {
            OnLeftCancelClickEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnRightClick(InputAction.CallbackContext context) { 
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnRightStartedClickEvent?.Invoke();
        }

        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled) {
            OnRightCanceledClickEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowStats(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnShowStatsEvent?.Invoke();
        }
    }
    void PlayerInput.IPlayerActions.OnShowEnemyStats(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnShowEnemyStatsEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowActionsWindow(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnShowActionsWindowEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowNewsWindow(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnShowNewsWindowEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnZoom(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnZoomEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnPauseGame(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnPauseEvent?.Invoke();
            ChangeGameMode(GameModes.UI);
        }
    }

    public void OnStopResumeTime(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnStopResumeTimeEvent?.Invoke();
        }
    }

    public void OnIncrementTime(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnIncrementTimeEvent?.Invoke();
        }
    }

    public void OnDecrementTime(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnDecrementTimeEvent?.Invoke();
        }
    }

    #endregion

    #region IUIActions Implementation
    public void OnResumeGame(InputAction.CallbackContext context) {
        if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
            OnResumeEvent?.Invoke();
            ChangeGameMode(GameModes.Game);
        }
    }

	void PlayerInput.IPlayerActions.OnSetTimeScale(InputAction.CallbackContext context) {
		if(context.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
        
        string controlName = context.control.name;
        
        if (int.TryParse(controlName, out int scaleIndex)) {
            OnSetTimeScale?.Invoke(scaleIndex); 
        }
    }
	}

	#endregion
}