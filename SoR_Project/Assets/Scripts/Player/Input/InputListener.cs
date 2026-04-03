using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputListener", menuName = "SOR/Input/InputListener")]
public class InputListener : ScriptableObject, PlayerInput.IPlayerActions, PlayerInput.IUIMenuActions {

    #region EVENTS

    /// <summary>Se invoca cuando el jugador se mueve</summary>
    public Action<Vector2> OnMoveEvent;
    /// <summary>Se invoca cuando el botón izquierdo del ratón se presiona</summary>
    public Action OnLeftStartClickEvent;
    /// <summary>Se invoca cuando el botón izquierdo del ratón se suelta</summary>
    public Action OnLeftCancelClickEvent;
    /// <summary>Se invoca cuando el botón derecho del ratón se presiona</summary>
    public Action OnRightStartedClickEvent; 
    /// <summary>Se invoca cuando el botón derecho del ratón se suelta</summary>
    public Action OnRightCanceledClickEvent; 
    /// <summary>Se invoca cuando se ejecuta la entrada de mostrar estadísticas</summary>
    public Action OnShowStatsEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de mostrar estadísticas enemigas</summary>
    public Action OnShowEnemyStatsEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de mostrar ventana de acciones</summary>
    public Action OnShowActionsWindowEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de mostrar ventana de noticias</summary>
    public Action OnShowNewsWindowEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de zoom</summary>
    public Action<Vector2> OnZoomEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de pausa</summary>
    public Action OnPauseEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de reanudación</summary>
    public Action OnResumeEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de detener/reanudar tiempo</summary>
    public Action OnStopResumeTimeEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de incrementar tiempo</summary>
    public Action OnIncrementTimeEvent;
    /// <summary>Se invoca cuando se ejecuta la entrada de decrementar tiempo</summary>
    public Action OnDecrementTimeEvent;
    /// <summary>Se invoca cuando se establece la escala de tiempo</summary>
    public Action<int> OnSetTimeScale;

    #endregion

    private PlayerInput _playerInput;

    private void OnEnable() {
        if (_playerInput == null) {
            _playerInput = new PlayerInput();

            _playerInput.Player.SetCallbacks(this);
            _playerInput.UIMenu.SetCallbacks(this);

            ChangeGameMode(SOR_Enums.GameModes.Game);
        }
    }

    void OnDisable() {
        _playerInput.Player.Disable();
    }

    public void ChangeGameMode(SOR_Enums.GameModes gMod) {
        switch (gMod) {
            case SOR_Enums.GameModes.Game:
                _playerInput.Player.Enable();
                _playerInput.UIMenu.Disable();
                break;
            case SOR_Enums.GameModes.UI:
                _playerInput.Player.Disable();
                _playerInput.UIMenu.Enable();
                break;
        }
    }

    private bool IsPhasePerformed(InputAction.CallbackContext context) => context.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
    private bool IsPhaseCanceled(InputAction.CallbackContext context) => context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled;
    private bool IsPointerOverUI() {
    if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Si hay resultados, hay UI bloqueando
        return results.Count > 0;
    }

    #region IPlayerActions Implementation
    void PlayerInput.IPlayerActions.OnMove(InputAction.CallbackContext context) {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void PlayerInput.IPlayerActions.OnLeftClick(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context) && !IsPointerOverUI()) {
            OnLeftStartClickEvent?.Invoke();
        }
        if (IsPhaseCanceled(context)) {
            OnLeftCancelClickEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnRightClick(InputAction.CallbackContext context) { 
        if (IsPhasePerformed(context)) {
            OnRightStartedClickEvent?.Invoke();
        }
        if (IsPhaseCanceled(context)) {
            OnRightCanceledClickEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowStats(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnShowStatsEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowEnemyStats(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnShowEnemyStatsEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowActionsWindow(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnShowActionsWindowEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnShowNewsWindow(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnShowNewsWindowEvent?.Invoke();
        }
    }

    void PlayerInput.IPlayerActions.OnZoom(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnZoomEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnPauseGame(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnPauseEvent?.Invoke();
            ChangeGameMode(SOR_Enums.GameModes.UI);
        }
    }

    public void OnStopResumeTime(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnStopResumeTimeEvent?.Invoke();
        }
    }

    public void OnIncrementTime(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnIncrementTimeEvent?.Invoke();
        }
    }

    public void OnDecrementTime(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnDecrementTimeEvent?.Invoke();
        }
    }

    #endregion

    #region IUIActions Implementation
    public void OnResumeGame(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            OnResumeEvent?.Invoke();
            ChangeGameMode(SOR_Enums.GameModes.Game);
        }
    }

    void PlayerInput.IPlayerActions.OnSetTimeScale(InputAction.CallbackContext context) {
        if (IsPhasePerformed(context)) {
            string controlName = context.control.name;
            if (int.TryParse(controlName, out int scaleIndex)) {
                OnSetTimeScale?.Invoke(scaleIndex);
            }
        }
    }

    #endregion
}