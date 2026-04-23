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
    private TimeManager _timeManager;
    private ProvinceData _currentData;
   [SerializeField] private SO_Province soProvince;

    
    // SO -> TO INFO
    [SerializeField, ShowOnly] private string provinceId;
    [SerializeField, ShowOnly] private string provinceName;
    [SerializeField, ShowOnly] private int population  = 1;
    [SerializeField, ShowOnly] private string provinceType;
    public string ProvinceId => provinceId;
    public string ProvinceName => provinceName;
    public int Population => population;
    public string ProvinceType => provinceType;

    public float stability {
        get { return Mathf.Clamp(_currentData.stability, 0, 100) ;}
        set {_currentData.stability = Mathf.Clamp(value, 0, 100); }
    }

    public float popularity {
        get {return Mathf.Clamp(_currentData.popularity, 0, soProvince.provincePopulation) ;}
        set { _currentData.popularity = Mathf.Clamp(value, 0, soProvince.provincePopulation); }
    }
    public float aligned {
        get {return Mathf.Clamp( _currentData.aligned, 0, _currentData.popularity) ;} 
        set { _currentData.aligned = Mathf.Clamp(value, 0, _currentData.popularity); }
    }
    public float affiliates {
        get {return Mathf.Clamp( _currentData.affiliates, 0, _currentData.aligned) ;} 
        set { _currentData.affiliates = Mathf.Clamp(value, 0, _currentData.aligned); }
    }  

    void OnDisable() {
        //_timeManager.OnDayPassedEvent-=debugShowInfo;
    }

    public void InitProvinceData(SO_Province soProvince) {
        this.soProvince = soProvince;
        _timeManager = ServiceLocator.Get<TimeManager>();
        //_timeManager.OnDayPassedEvent+=debugShowInfo;

        if (soProvince != null) {
            provinceId = soProvince.provinceId;
            provinceName = soProvince.provinceName;
            population = soProvince.provincePopulation;
            provinceType = soProvince.provinceType;
        }
    }
    public void InsertData(ProvinceData data) => _currentData = data;
    public string GetProvinceId() => soProvince.provinceId;

    void debugShowInfo() {
        if(_currentData == null) return;
        popularity += SeedRandom.RangeInt(SeedCategory.TEST, 100, 1000);
        aligned += SeedRandom.RangeInt(SeedCategory.TEST, -10, 500);
        affiliates += SeedRandom.RangeInt(SeedCategory.TEST, -10, 200);
    }

}
