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
   // private MeshRenderer _meshRenderer;
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
        _timeManager.OnDayPassedEvent-=debugShowInfo;
    }

    

    public void InitProvinceData(SO_Province soProvince) {
        this.soProvince = soProvince;
        _timeManager = ServiceLocator.Get<TimeManager>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager.OnDayPassedEvent+=debugShowInfo;

        if (soProvince != null) {
            provinceName = soProvince.provinceName;
            population = soProvince.provincePopulation;
            provinceType = soProvince.provinceType;
        }
    }
    public void InsertData(ProvinceData data) => _currentData = data;

    void debugShowInfo() {
        if(_currentData == null) return;
        popularity += 1024;
        aligned += 512;
        affiliates += 256;
    }

	public void LeftClickInteract() {
        //_provinceManager.SelectProvinceByColor(this);
        // _meshRenderer.material.color = Color.green;
	}

	public void OnHover() {
        //if(_provinceManager.selectedProvince == this) return;
		// _meshRenderer.material.color = Color.blue;
	}

	public void OnDeselect() {
		//_provinceManager.SelectProvinceByColor(null);
        // _meshRenderer.material.color = Color.white;
	}

	public void OnUnhover() {   
        //if(_provinceManager.selectedProvince == this) return;
        // _meshRenderer.material.color = Color.white;
		
	}

    public bool IsInitialized() => _currentData != null;
    public string GetProvinceId() => soProvince.provinceId;
}
