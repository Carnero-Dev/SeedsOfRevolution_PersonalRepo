using UnityEngine;
using System;
//TODO: Refactorizar timeManager para usar TimeData y asignar geters y seters
/// <summary>
/// Clase que gestiona el tiempo de juego respecto al tiempo real y ejecuta acciones con respecto al paso de días, meses, años, etc
/// Author: Carlos Carnero Cabrera
/// </summary>
public class TimeManager : MonoBehaviour {
    private TimeManagerData _data => GameDataService.Current.gameTime;
    private SO_CalendarConfig _calendar;
    private int _currentAbsDay; // Día relativo al inicio
    public int CurrentAbsDay => _currentAbsDay;
    [SerializeField] 
    [Tooltip("Cada segundo en la vida real es X horas en el juego")]
        private int _TIMESCALE = 300; // Controla la velocidad del mundo con respecto a las fechas
    private int [] _timesScales = new int [4]; // Establece las velocidades del juego (EN ORDEN INCLUYENDO PAUSA)
    private int _currentTimeScaleIndex = 1;  // Establece cual es la velocidad actual
    private int _lastTimeScaleIndex = 1;  // Establece cual es la velocidad actual
    private bool _timeInputEnabled { get; set; } = true; // Controla si el jugador puede cambiar la velocidad del tiempo

    // Actions
    public Action OnHourPassedEvent;
    public Action OnDayPassedEvent;
    public Action OnMonthPassedEvent;
    public Action OnYearPassedEvent;

    public Action OnGameTimePaused;
    public Action OnGameTimeReanudated;
    public Action OnGameTimeScaleChanged;

    public void Init(SO_CalendarConfig calendarConfig) {
        _calendar = calendarConfig;
        // Si el año es el por defecto (-1) aplicamos la fecha de inicio del mapa
        if (_data.year < 0) { 
            _calendar.ApplyStartDateToData(_data);
        }
        _currentAbsDay = _calendar.GetRelativeAbsDay(_data.day, _data.month, _data.year);
    }
    void Start() {
        PauseReanudeTime();
    }

    void Update() {
        CalculateTime();
    }

	void Awake()
	{		
        _timesScales[0] = 0;
        _timesScales[1] = _TIMESCALE;
        _timesScales[2] = _TIMESCALE * 2;
        _timesScales[3] = _TIMESCALE * 3;
	}

    #region Calculate Time
    void CalculateTime() {
        if (_currentTimeScaleIndex == 0) return;

        _data.minute += Time.deltaTime * _TIMESCALE;

        if (_data.minute >= 60) {
            _data.minute = 0;
            _data.hour++;
            OnHourPassedEvent?.Invoke();

            if (_data.hour >= 24) {
                _data.hour = 0;
                ProcessDayPass();
            }
        }
    }

    void ProcessDayPass() {
        _data.day++;
        _currentAbsDay++; // Incremento lineal simple

        int maxDays = _calendar.GetDaysInMonth(_data.month, _data.year);

        if (_data.day > maxDays) {
            _data.day = 1;
            _data.month++;
            OnMonthPassedEvent?.Invoke();

            if (_data.month > _calendar.months.Length) {
                _data.month = 1;
                _data.year++;
                OnYearPassedEvent?.Invoke();
            }
        }
        OnDayPassedEvent?.Invoke();
    }
    #endregion
    #region Time Controller
    private void ChangeTimeScale(int newIndex) {
        if (_timeInputEnabled == false) return;
        _lastTimeScaleIndex = _currentTimeScaleIndex;
        _currentTimeScaleIndex = newIndex;
        _TIMESCALE = _timesScales[_currentTimeScaleIndex];
        OnGameTimeScaleChanged?.Invoke();
    }
    public void AccelerateTime() {
        if (_currentTimeScaleIndex < _timesScales.Length - 1) {
            ChangeTimeScale(_currentTimeScaleIndex + 1);
        }
    }

    public void DecreaseTime() {
        if (_currentTimeScaleIndex > 0) {
            ChangeTimeScale(_currentTimeScaleIndex - 1);
        }
    }
    /// <summary>
    /// Pausa (false) o reanuda (true) el tiempo del juego, guardando la velocidad actual para retomarla al reanudar
    /// </summary>
    public void PauseReanudeTime(bool pause) {
        if (_timeInputEnabled == false) return;
        if (pause) {
            ChangeTimeScale(_lastTimeScaleIndex);
            OnGameTimeReanudated?.Invoke();
        } else {
            _lastTimeScaleIndex = _currentTimeScaleIndex;
            ChangeTimeScale(0);
            OnGameTimePaused?.Invoke();
        }
    }
    /// <summary>
    /// Intercala entre pausar y reanudar el tiempo del juego según el estado actual, guardando la velocidad actual para retomarla al reanudar
    /// </summary>
    public void PauseReanudeTime() {
        if (_timeInputEnabled == false) return;
        if (_currentTimeScaleIndex != 0) {
            _lastTimeScaleIndex = _currentTimeScaleIndex;
            ChangeTimeScale(0);
            OnGameTimePaused?.Invoke();
        } else {
            ChangeTimeScale(_lastTimeScaleIndex);
            OnGameTimeReanudated?.Invoke();
        }
    }

    public void SetTimeScale(int index) {
        if (_timeInputEnabled == false) return;
        int targetIndex = Mathf.Clamp(index, 1, _timesScales.Length - 1);
        ChangeTimeScale(targetIndex);
    }
    public int GetCurrentTimeScale() => _currentTimeScaleIndex;
    /// <summary>
    /// Habilita o deshabilita la capacidad del jugador de cambiar la velocidad del tiempo
    /// </summary> <param name="value">true para habilitar, false para deshabilitar</param>
    public bool SetTimeInputActive(bool value) {
        _timeInputEnabled = value;
        return _timeInputEnabled;
    }
    public string GetMonthName(int month) => _calendar.months[month - 1].name;

    #endregion
}
