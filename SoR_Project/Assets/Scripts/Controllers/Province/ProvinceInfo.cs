using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
/// <summary>
/// Script que se encarga de gestionar la información de la provincia y devolver sus valores
/// Author: Carlos Carnero Cabrera
/// </summary>
public class ProvinceInfo : MonoBehaviour, IInteractable
{
    // REFERENCES
    private TimeManager _timeManager;
    private ProvinceManager _provinceManager;
    private MeshRenderer _meshRenderer;
   [SerializeField] private SO_Province soProvince;
    private ProvinceData _currentData;

    
    // SO -> TO INFO
    [SerializeField, ShowOnly] private string provinceName;
    [SerializeField, ShowOnly] private int population  = 1;
    [SerializeField, ShowOnly] private string provinceType;
    public string ProvinceName => provinceName;
    public int Population => population;
    public string ProvinceType => provinceType;
    
    // PARAMETERS
    // public float popularity {
    //     get {return _currentPopularity =  Mathf.Clamp(value: _currentPopularity, 0, population);}
    //     set { _currentPopularity = Mathf.Clamp(value, 0, population);}}
    // public float affiliates {
    //     get {return _currentAffiliates = Math.Clamp(value: _currentAffiliates, 0, population);} 
    //     set { _currentAffiliates = Mathf.Clamp(value, 0, population);}}
    // [SerializeField, DynamicRange(0, "population"), Tooltip("Clampeado a los habitantes")] 
    // private  float _currentPopularity;
    // [SerializeField, DynamicRange(0, "population"), Tooltip("Clampeado a los habitantes")] 
    // private  float _currentAffiliates;
    [Range (0, 100)] public float determination;

    public float popularity {
        get {return Mathf.Clamp(_currentData.popularity, 0, soProvince._population) ;}
        set { _currentData.popularity = value; }
    }
    public float affiliates {
        get {return Mathf.Clamp( _currentData.affiliates, 0, soProvince._population) ;} 
        set { _currentData.affiliates = value; }
    }

    private void OnValidate() {
        _meshRenderer = GetComponent<MeshRenderer>();
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
        _timeManager.OnDayPassedEvent-=debugShowInfo;
    }

    void Start() {
        _meshRenderer.material.color = Color.white;
        _timeManager = ServiceLocator.Get<TimeManager>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager.OnDayPassedEvent+=debugShowInfo;

        _currentData = _provinceManager.GetProvinceState(soProvince._provinceId);
        if(_currentData != null) {
            // ApplyData
        }
    }

    void debugShowInfo() {
        if(_currentData == null) return;
        popularity += 1024;
        affiliates += 512;
        // _currentPopularity += 1024;
        // _currentAffiliates += 512;
    }

	public void LeftClickInteract() {
        _provinceManager.SelectProvince(this);
        _meshRenderer.material.color = Color.green;
	}

	public void OnHover() {
        if(_provinceManager.selectedProvince == this) return;
		_meshRenderer.material.color = Color.blue;
	}

	public void OnDeselect() {
		_provinceManager.SelectProvince(null);
        _meshRenderer.material.color = Color.white;
	}

	public void OnUnhover() {   
        if(_provinceManager.selectedProvince == this) return;
        _meshRenderer.material.color = Color.white;
		
	}
}
