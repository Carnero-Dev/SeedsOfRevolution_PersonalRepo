using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
/// <summary>
/// Script que se encarga de gestionar la información de la provincia y devolver sus valores
/// Author: Carlos Carnero Cabrera
/// </summary>
public class ProvinceInfo : MonoBehaviour
{
    // REFERENCES
    private TimeManager timeManager;
    public SO_Province soProvince;
    
    // SO -> TO INFO
    [SerializeField, ShowOnly] private string provinceName;
    [SerializeField, ShowOnly] private int population  = 1;
    [SerializeField, ShowOnly] private string provinceType;
    public string ProvinceName => provinceName;
    public int Population => population;
    public string ProvinceType => provinceType;
    
    // PARAMETERS
    public float popularity {
        get {return _currentPopularity =  Mathf.Clamp(value: _currentPopularity, 0, population);}
        set { _currentPopularity = Mathf.Clamp(value, 0, population);}}
    public float affiliates {
        get {return _currentAffiliates = Math.Clamp(value: _currentAffiliates, 0, population);} 
        set { _currentAffiliates = Mathf.Clamp(value, 0, population);}}
    [SerializeField, DynamicRange(0, "population"), Tooltip("Clampeado a los habitantes")] 
    private  float _currentPopularity;
    [SerializeField, DynamicRange(0, "population"), Tooltip("Clampeado a los habitantes")] 
    private  float _currentAffiliates;
    [Range (0, 100)] public float determination;


    private void OnValidate() {
        if (soProvince != null) {
            provinceName = soProvince._name;
            population = soProvince._population;
            provinceType = soProvince._provinceType;
        } else {
            provinceName = "(Unamed)";
            population = 1;
            provinceType = "Default";
        }
    }    

    void OnDisable() {
        timeManager.OnDayPassedEvent-=debugShowInfo;
    }

    void Start() {
        timeManager = ServiceLocator.Instance.GetService<TimeManager>();
        timeManager.OnDayPassedEvent+=debugShowInfo;
    }

    void debugShowInfo() {
        _currentPopularity += 1024;
        _currentAffiliates += 512;
    }
}
