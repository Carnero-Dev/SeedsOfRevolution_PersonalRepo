using UnityEngine;
using System;

/// <summary>
/// Clase que gestiona el tiempo de juego respecto al tiempo real y ejecuta acciones con respecto al paso de días, meses, años, etc
/// Author: Carlos Carnero Cabrera
/// </summary>
public class TimeManager : MonoBehaviour {
    private TimeManagerData _data => GameDataService.Current.gameTime;
    [SerializeField] 
    [Tooltip("Cada segundo en la vida real es X horas en el juego")]
        private int _TIMESCALE = 300; // Controla la velocidad del mundo con respecto a las fechas
    private int [] _timesScales = new int [4]; // Establece las velocidades del juego (EN ORDEN INCLUYENDO PAUSA)
    private int _currentTimeScaleIndex = 1;  // Establece cual es la velocidad actual
    private int _lastTimeScaleIndex = 1;  // Establece cual es la velocidad actual

    // Actions
    public Action OnHourPassedEvent;
    public Action OnDayPassedEvent;
    public Action OnMonthPassedEvent;
    public Action OnYearPassedEvent;

    public Action OnGameTimePaused;
    public Action OnGameTimeReanudated;
    public Action OnGameTimeScaleChanged;

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
        // Comprueba si ha pasado 24 horas para pasar de dia y resetear el contador  
        _data.minute += Time.deltaTime * _TIMESCALE;
        if(_data.minute >= 59) { 
            if(_data.hour >= 23 || _data.hour < 0) {
                _data.minute = 0;
                _data.hour = 0;
                OnHourPassedEvent?.Invoke();
                _data.day = CheckMonth() ? 1 : _data.day + 1; // Checkea si ha pasado de mes para resetear el dia        
                OnDayPassedEvent?.Invoke();
            } else
            _data.hour++; _data.minute = 0; OnHourPassedEvent?.Invoke(); 
        }   
    }

    bool CheckMonth(){
        // Formatear valores mínimos
        if (_data.month < 1) _data.month = 1;
        if (_data.day < 1) _data.day = 1;

        if ((_data.month == 1 || _data.month == 3 || _data.month == 5 || _data.month == 7 || _data.month == 8 || _data.month == 10 || _data.month == 12) 
        && _data.day >= 31) {
            // Checkea si pasamos año nuevo para resetear mes y avanzar de año
            if(_data.month == 12) {
                _data.month = 1;
                _data.year++;
                OnYearPassedEvent?.Invoke();
            } else {
                _data.month++;
            }
            OnMonthPassedEvent?.Invoke();
            return true;
        }
        if ((_data.month == 4 || _data.month == 6 || _data.month == 9 || _data.month == 11) 
        && _data.day >= 30) {
            _data.month++;
            OnMonthPassedEvent?.Invoke();
            return true;
        }
        if (_data.month == 2 && _data.day >= 28) {
            _data.month++;
            OnMonthPassedEvent?.Invoke();
            return true;
        }
        return false;
    }
    #endregion
    #region Time Controller
    private void ChangeTimeScale(int newIndex) {
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
    public void PauseReanudeTime(bool pause) {
        if (pause) {
            ChangeTimeScale(_lastTimeScaleIndex);
            OnGameTimeReanudated?.Invoke();
        } else {
            _lastTimeScaleIndex = _currentTimeScaleIndex;
            ChangeTimeScale(0);
            OnGameTimePaused?.Invoke();
        }
    }

    public void PauseReanudeTime() {
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
        int targetIndex = Mathf.Clamp(index, 1, _timesScales.Length - 1);
        ChangeTimeScale(targetIndex);
    }
    public int GetCurrentTimeScale() => _currentTimeScaleIndex;

    #endregion
}
