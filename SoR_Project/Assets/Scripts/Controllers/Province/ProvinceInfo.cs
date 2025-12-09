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

    public float stability {
        get { return Mathf.Clamp(_currentData.stability, -100, 100) ;}
        set {_currentData.stability = Mathf.Clamp(value, -100, 100); }
    }

    public float popularity {
        get {return Mathf.Clamp(_currentData.popularity, 0, soProvince._population) ;}
        set { _currentData.popularity = Mathf.Clamp(value, 0, soProvince._population); }
    }
    public float aligned {
        get {return Mathf.Clamp( _currentData.aligned, 0, _currentData.popularity) ;} 
        set { _currentData.aligned = Mathf.Clamp(value, 0, _currentData.popularity); }
    }
    public float affiliates {
        get {return Mathf.Clamp( _currentData.affiliates, 0, _currentData.aligned) ;} 
        set { _currentData.affiliates = Mathf.Clamp(value, 0, _currentData.aligned); }
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
    }

    void debugShowInfo() {
        if(_currentData == null) return;
        popularity += 1024;
        aligned += 512;
        affiliates += 256;
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
