using UnityEngine;
using System;

/// <summary>
/// Clase que gestiona el tiempo de juego respecto al tiempo real y ejecuta acciones con respecto al paso de días, meses, años, etc
/// Author: Carlos Carnero Cabrera
/// </summary>
public class TimeManager : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Cada segundo en la vida real es X horas en el juego")]
        private int _TIMESCALE = 20; // Controla la velocidad del mundo con respecto a las fechas
    private int [] _timesScales = new int [4]; // Establece las velocidades del juego (EN ORDEN INCLUYENDO PAUSA)
    public int currentTimeScaleIndex {get; private set;} = 1;  // Establece cual es la velocidad actual

    public double hour {get; private set;}
    public int day {get { return _currentDay; } private set { _currentDay = Math.Clamp(value, 1, 31);}}
    public int month {get { return _currentMonth; } private set { _currentMonth = Math.Clamp(value, 1, 12);}}
    public int year { get { return _currentYear; } private set { _currentYear = Math.Clamp(value, 0, 9999); }}

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
        day = CheckMonth() ? 1 : day + 1; // Checkea si ha pasado de mes para resetear el dia
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
        hour += Time.deltaTime * _TIMESCALE;         
        // Comprueba si ha pasado 24 horas para pasar de dia y resetear el contador  
        if(hour > 24) {
            hour = 0;
            OnDayPassedEvent?.Invoke();
            day = CheckMonth() ? 1 : day + 1; // Checkea si ha pasado de mes para resetear el dia        
        }
    }

    bool CheckMonth(){
        if ((month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) 
        && day >= 31) {
            // Checkea si pasamos año nuevo para resetear mes y avanzar de año
            if(month == 12) {
                month = 1;
                year++;
                OnYearPassedEvent?.Invoke();
            } else {
                month++;
            }
            OnMonthPassedEvent?.Invoke();
            return true;
        }
        if ((month == 4 || month == 6 || month == 9 || month == 11) 
        && day >= 30) {
            month++;
            OnMonthPassedEvent?.Invoke();
            return true;
        }
        if (month == 2 && day >= 28) {
            month++;
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
