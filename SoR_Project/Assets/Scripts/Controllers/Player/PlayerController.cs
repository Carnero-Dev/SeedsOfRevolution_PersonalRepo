using System;
using UnityEngine;

// TODO: suavizar movimiento con wasd y dejarlo como opción para poder activarlo y desactivarlo

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("REFERENCES")]
    public InputListener input;
    public Camera playerCamera;
    private TimeManager _timeManager;

    [Header("INPUT SETTINGS")]

    //! DEPRECATED LEFT HOLD
    // [SerializeField, Tooltip("Tiempo que debe pasar desde que el jugador hace click para que se ejecute el Hold")]
    // private float _holdTimeThreshold = 0.2f;

    [Header("MOVEMENT SETTINGS")]

    [SerializeField, Tooltip("Velicidad de movimiento de la cámara")]
    private float _moveSpeed = 5f;
    [SerializeField, Tooltip("Multiplicador de velocidad aplicado cuándo el jugador está lejos del suelo. El valor interpola desde 1 cuándo está a nivel de suelo al número que se pone aquí.")]
    private float speedMultiplier = 2f;

    [Header("MOUSE MOVEMENT SETTINGS")]
    [SerializeField, Tooltip("Velocidad de movimiento al arrastrar el mouse")]
    private float _dragMoveSpeed = 10f;

    [Header("ZOOM SETTINGS")]
    [SerializeField, Tooltip("Velocidad del zoom.")]
    private float _zoomSpeed = 10f;
    [SerializeField, Tooltip("Suavizado de movimiento en zoom. Cuánto menor el valor, más suavizado el movimiento")]
    private float _zoomSmoothness = 5f;
    [SerializeField, Tooltip("Límites de movimiento en ejes X, Z e Y positivo. El Y negativo se controla solo")]
    [VectorLabels("LimitX", "LimitY (+)", "LimitZ")]
    private Vector3 _limits;
    [SerializeField, Tooltip("Offset de distancia al suelo. Cuánto más alto, más lejos se queda del suelo al bajar")]
    private float _groundDistanceOffset = 1f;
    [SerializeField, Tooltip("LayerMask del terreno")]
    private LayerMask _groundLayerMask;

    [Header("CAMERA ROTATION SETTINGS")]
    [SerializeField, Tooltip("Suavizado de roatción de la cámara")]
    private float _rotationSmoothness = 5f;
    [SerializeField, Tooltip("Límites de ángulo de la cámara")]
    [VectorLabels("MaxRotation", "MinRotation")]
    private Vector2 limitTiltAngles = new Vector2(0f, 30f);

    [Header("INTERACTION SETTINGS")]

    [SerializeField, Tooltip("LayerMask de interacción")]
    private LayerMask _interactionLayer;
    [SerializeField, Tooltip("Distancia máxima a la que se puede interactuar")]
    private float _interactionDistance = 500;
    
    [SerializeField, Tooltip("LayerMask del terreno o fondo, para saber cuándo deseleccionar")]
    private LayerMask _terrainLayerMask; 

    [Header("DEBUG")]
    [SerializeField] private bool _enableDebug;
    [SerializeField] private bool _debugInteractionRay;
    [SerializeField] private bool _debugLimits;

    private Rigidbody _rb;
    private bool _rightClickHold; 
    //! DEPRECATED LEFT HOLD
    // private bool _leftClickHold;
    // private float _holdTimer;
    private float _targetHeight;
    private float _scrollInput;
    private Vector3 _groundHitPoint;
    private Vector3 _mouseInitialPosition; 
    private bool _isDragging; 
    private float _mouseDragThreshold = 10f;
    private IInteractable _hoveredInteractable;
    private IInteractable _interactedItem;
    private IInteractable _selectedInteractable;

    #region DEBUG
    void OnDrawGizmos() {
        if (_enableDebug) {
            if (_debugInteractionRay && playerCamera != null) {
                Gizmos.color = Color.red;
                Vector3 to = playerCamera.transform.forward * _interactionDistance;
                DrawDebugRay(transform.position, to, playerCamera.transform.forward, _interactionDistance, Color.yellow);
            }

            if (_debugLimits) {
                DrawDebugRay(transform.position, transform.right * _limits.x, transform.right, _limits.x, Color.red);
                DrawDebugRay(transform.position, -transform.right * _limits.x, -transform.right, _limits.x, Color.red);
                DrawDebugRay(transform.position, transform.forward * _limits.z, transform.forward, _limits.z, Color.blue);
                DrawDebugRay(transform.position, -transform.forward * _limits.z, -transform.forward, _limits.z, Color.blue);
                DrawDebugRay(transform.position, transform.up * _limits.y, transform.up, _limits.y, Color.green);
            }
        }
    }

    private void DrawDebugRay(Vector3 from, Vector3 to, Vector3 direction, float distance, Color color) {
        Gizmos.color = color;
        Gizmos.DrawRay(from, to);
        Gizmos.DrawSphere(from + direction * distance, 0.1f);
    }

    #endregion

    #region ENABLE / DISABLE

    void OnEnable() {
        input.OnMoveEvent += HandleMove;
        input.OnLeftStartClickEvent += HandlePerformLeftClick;
        input.OnLeftCancelClickEvent += HandleCancelLeftClick;
        input.OnRightStartedClickEvent += HandlePerformRightClick; 
        input.OnRightCanceledClickEvent += HandleCancelRightClick;
        input.OnShowStatsEvent += HandleShowStats;
        input.OnShowEnemyStatsEvent += HandleShowEnemyStats;
        input.OnShowActionsWindowEvent += HandleShowActionsWindow;
        input.OnShowNewsWindowEvent += HandleShowNewsWindow;
        input.OnZoomEvent += HandleZoom;
        input.OnPauseEvent += HandlePauseGame;
        input.OnResumeEvent += HandleResumeGame;
        input.OnStopResumeTimeEvent += HandleStopResumeTime;
        input.OnIncrementTimeEvent += HandleIncrementTime;
        input.OnDecrementTimeEvent += HandleDecrementTime;
        input.OnSetTimeScale += HandleSetTimeScale;
    }

    void OnDisable() {
        input.OnMoveEvent -= HandleMove;
        input.OnLeftStartClickEvent -= HandlePerformLeftClick;
        input.OnLeftCancelClickEvent -= HandleCancelLeftClick;
        input.OnRightStartedClickEvent -= HandlePerformRightClick; 
        input.OnRightCanceledClickEvent -= HandleCancelRightClick; 
        input.OnShowStatsEvent -= HandleShowStats;
        input.OnShowEnemyStatsEvent -= HandleShowEnemyStats;
        input.OnShowActionsWindowEvent -= HandleShowActionsWindow;
        input.OnShowNewsWindowEvent -= HandleShowNewsWindow;
        input.OnZoomEvent -= HandleZoom;
        input.OnPauseEvent -= HandlePauseGame;
        input.OnResumeEvent -= HandleResumeGame;
        input.OnStopResumeTimeEvent -= HandleStopResumeTime;
        input.OnIncrementTimeEvent -= HandleIncrementTime;
        input.OnDecrementTimeEvent -= HandleDecrementTime;
        input.OnSetTimeScale -= HandleSetTimeScale;
    }

    #endregion

    void Awake() {
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        _rb = GetComponent<Rigidbody>();
    }

    void Start() {
        _targetHeight = transform.position.y;
        _timeManager = ServiceLocator.Get<TimeManager>();
    }

    void Update() {
        HoverRay();
        // CheckLeftHoldForDrag();
        // LeftCLickHoldTimer();
        CalculateZoomHeight();
        CheckGroundDistance();
        Zoom();
    }

    void FixedUpdate() {
        MouseMovement(); // 
        AdjustCameraTilt();
    }

    private void CalculateZoomHeight() {
        if (Math.Abs(_scrollInput) > 0.01f) {
            _targetHeight -= _scrollInput * _zoomSpeed;
            _targetHeight = Mathf.Clamp(_targetHeight, _groundHitPoint.y + _groundDistanceOffset, _limits.y);
        }

        _scrollInput = 0;
    }

    private void Zoom() {
        float newHeight = Mathf.Lerp(_rb.position.y, _targetHeight, Time.deltaTime * _zoomSmoothness);

        float currentX = _rb.position.x;
        float currentZ = _rb.position.z;

        Vector3 potentialNewPosition = new Vector3(currentX, newHeight, currentZ);

        float clampedX = Mathf.Clamp(potentialNewPosition.x, -_limits.x, _limits.x);
        float clampedZ = Mathf.Clamp(potentialNewPosition.z, -_limits.z, _limits.z);

        Vector3 finalNewPosition = new Vector3(clampedX, potentialNewPosition.y, clampedZ);

        _rb.MovePosition(finalNewPosition);
    }

    private void CheckGroundDistance() {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayerMask))
        {
            _groundHitPoint = hit.collider.transform.position;
        }
    }

    private IInteractable GetInteractableUnderMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactionLayer)){
            return hit.collider.gameObject.GetComponent<IInteractable>();
        }

        if (Physics.Raycast(ray, out hit, _interactionDistance, _terrainLayerMask))
        {
            return null; 
        }

        return null;
    }

    private void AdjustCameraTilt() {
        if (playerCamera == null) return;

        float normalizedHeight = Mathf.InverseLerp(
            _groundHitPoint.y + _groundDistanceOffset,
            _limits.y,
            transform.position.y
        );

        float targetAngle = Mathf.Lerp(limitTiltAngles.y, limitTiltAngles.x, normalizedHeight);

        Quaternion targetRotation = Quaternion.Euler(targetAngle, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);

        playerCamera.transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSmoothness);
    }
    //! DEPRECATED LEFT HOLD
    // private void CheckLeftHoldForDrag()
    // {
    //     // 1. Si no estamos presionando el clic izquierdo o si ya estamos arrastrando, salir.
    //     if (!_leftClickHold || _isDragging) return;
        
    //     // 2. Comprobamos si el clic izquierdo se ha mantenido lo suficiente.
    //     _holdTimer += Time.deltaTime;

    //     if (_holdTimer >= _holdTimeThreshold)
    //     {
    //         // 3. Activamos el Drag
    //         _isDragging = true;
    //         // Capturamos la posición inicial del mouse para el movimiento
    //         _mouseInitialPosition = Input.mousePosition; 
            
    //     }
    // }

    private void MouseMovement() {
        if (!_rightClickHold && !_isDragging) return;

        Vector3 currentMousePosition = Input.mousePosition;

        if (_rightClickHold)
        {
             float mouseDragDistance = Vector3.Distance(_mouseInitialPosition, currentMousePosition);
             if (mouseDragDistance > _mouseDragThreshold) {
                 _isDragging = true;
             }
             if (!_isDragging) return;
        }

        Vector3 deltaMouse = _mouseInitialPosition - currentMousePosition;
        Vector3 movement = new Vector3(deltaMouse.x, 0, deltaMouse.y) * _dragMoveSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = _rb.position + movement;
        newPosition = new Vector3(
            Mathf.Clamp(newPosition.x, -_limits.x, _limits.x),
            newPosition.y,
            Mathf.Clamp(newPosition.z, -_limits.z, _limits.z)
        );

        _rb.MovePosition(newPosition);
        // NOTA: Para un drag suave, la posición inicial del mouse debe actualizarse en cada frame de drag
        // o usar una lógica de arrastre basada en delta. Aquí usamos la actualización constante para mantener el arrastre.
        _mouseInitialPosition = currentMousePosition; 
    }

    #region Interaction

    private void HoverRay() {
        if (_isDragging) {
            if (_hoveredInteractable != null) {
                _hoveredInteractable.OnUnhover();
                _hoveredInteractable = null;
             }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        IInteractable previousHover = _hoveredInteractable;
        _hoveredInteractable = null;

        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactionLayer)) {
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            
            if (interactable != null) {
                _hoveredInteractable = interactable;
                
                if (previousHover != interactable) {
                    _hoveredInteractable.OnHover();
                }
            }
        }
        if (previousHover != null && previousHover != _hoveredInteractable) {
            previousHover.OnUnhover();
        }
    }
    //! DEPRECATED LEFT HOLD
    // private void LeftCLickHoldTimer() {
    //     if (_isDragging || !_leftClickHold) return;

    //     //_holdTimer += Time.deltaTime;
    //     if (_holdTimer >= _holdTimeThreshold) return;

    //    if (_interactedItem != null) {
    //         _interactedItem.OnLeftClickHold();
    //     }
    // }

    #endregion

    #region Input Event Handlers
    private void HandleMove(Vector2 inputValue) {
        float normalizedHeight = Mathf.InverseLerp(
            _groundHitPoint.y + _groundDistanceOffset,
            _limits.y,
            transform.position.y
        );

        float currentSpeedMultiplier = Mathf.Lerp(1f, speedMultiplier, normalizedHeight);

        Vector3 velocity = new Vector3(inputValue.x * _moveSpeed, 0, inputValue.y * _moveSpeed) * currentSpeedMultiplier;

        _rb.linearVelocity = velocity;
    }

    private void HandleZoom(Vector2 inputValue) {
        _scrollInput = inputValue.y;
    }

    private void HandlePerformLeftClick() {
        Debug.Log("Left Click START");
        // _leftClickHold = true;
        // _holdTimer = 0;
        
        if (_rightClickHold) return;

        IInteractable hitInteractable = GetInteractableUnderMouse();
        if (_selectedInteractable != null && _selectedInteractable != hitInteractable)
        {
            _selectedInteractable.OnDeselect(); 
            _selectedInteractable = null;
        }
        if (hitInteractable != null) {
            hitInteractable.LeftClickInteract(); 
            
            _interactedItem = hitInteractable;             
            _selectedInteractable = hitInteractable; 
        }
        else {
            _interactedItem = null;
        }
    }

    private void HandleCancelLeftClick() {
        if (_isDragging) {
             _isDragging = false;
        }         
        
        // _leftClickHold = false;
        // _holdTimer = 0;
        _interactedItem = null; 
    }

    private void HandlePerformRightClick() { 
        Debug.Log("Right Click START (Drag)");
        _rightClickHold = true;
        _mouseInitialPosition = Input.mousePosition;
    }

    private void HandleCancelRightClick() { 
        _rightClickHold = false;
        _isDragging = false;
    }

    private void HandleShowStats() { Debug.Log("Show Stats"); }
    private void HandleShowEnemyStats() { Debug.Log("Show Enemy Stats"); }
    private void HandleShowActionsWindow() { Debug.Log("Show Actions Menu"); }
    private void HandleShowNewsWindow() { Debug.Log("Show News Windows"); }
    private void HandleStopResumeTime() { _timeManager.PauseReanudeTime(); }
    private void HandleIncrementTime() { _timeManager.AccelerateTime(); }
    private void HandleDecrementTime() { _timeManager.DecreaseTime(); }
    private void HandlePauseGame() { Debug.Log("Pause Game"); }
    private void HandleResumeGame() { Debug.Log("Resume Game"); }
    private void HandleSetTimeScale(int index) { _timeManager.SetTimeScale(index); }

    #endregion
}