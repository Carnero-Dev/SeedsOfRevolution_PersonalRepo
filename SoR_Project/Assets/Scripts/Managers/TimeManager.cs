using UnityEngine;
using System;

/// <summary>
/// Clase que gestiona el tiempo de juego respecto al tiempo real y ejecuta acciones con respecto al paso de días, meses, años, etc
/// Author: Carlos Carnero Cabrera
/// </summary>
public class TimeManager : MonoBehaviour {
    private TimeManagerData _data => GameDataService.Current.timeManager;
    [SerializeField] 
    [Tooltip("Cada segundo en la vida real es X horas en el juego")]
        private int _TIMESCALE = 20; // Controla la velocidad del mundo con respecto a las fechas
    private int [] _timesScales = new int [4]; // Establece las velocidades del juego (EN ORDEN INCLUYENDO PAUSA)
    public int currentTimeScaleIndex {get; private set;} = 1;  // Establece cual es la velocidad actual

    [Header("Start Date")]
    [Range(1, 31)]
    [SerializeField] private int _currentDay;
    [Range(1, 12)]
    [SerializeField] private int _currentMonth;
    [Range(0, 9999)]
    [SerializeField] private int _currentYear;

    // Actions
    public Action OnDayPassedEvent;
    public Action OnMonthPassedEvent;
    public Action OnYearPassedEvent;

    void Start() {
        PauseTime();
    }

    void Update() {
        CalculateTime();
    }
    
    void OnValidate() {
        _timesScales[0] = 0;
        _timesScales[1] = _TIMESCALE;
        _timesScales[2] = _TIMESCALE * 2;
        _timesScales[3] = _TIMESCALE * 3;
    }

    #region Calculate Time
    void CalculateTime() {
        _data.hour += Time.deltaTime * _TIMESCALE;         
        // Comprueba si ha pasado 24 horas para pasar de dia y resetear el contador  
        if(_data.hour >= 23.99f || _data.hour < 0) {
            _data.hour = 0;
            OnDayPassedEvent?.Invoke();
            _data.day = CheckMonth() ? 1 : _data.day + 1; // Checkea si ha pasado de mes para resetear el dia        
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
    public void AccelerateTime() {        
        for (int i = 0; i < _timesScales.Length -1; i++) {
            if (currentTimeScaleIndex == i) {
                currentTimeScaleIndex++;
                _TIMESCALE = _timesScales[currentTimeScaleIndex];
                break;
            }            
       }
    }
    public void DecreaseTime() {
       for (int i = 1; i < _timesScales.Length; i++) {
            if (currentTimeScaleIndex == i) {
                currentTimeScaleIndex--;
                _TIMESCALE = _timesScales[currentTimeScaleIndex];
                break;
            }
       }
    }
    public void PauseTime(){
        currentTimeScaleIndex = 0;
        _TIMESCALE = _timesScales[currentTimeScaleIndex];
    }
    public void ReanudeTime(){
        if (currentTimeScaleIndex == 0) {
            currentTimeScaleIndex = 1;
        }
        _TIMESCALE = _timesScales[currentTimeScaleIndex];
    }    

    #endregion
}
