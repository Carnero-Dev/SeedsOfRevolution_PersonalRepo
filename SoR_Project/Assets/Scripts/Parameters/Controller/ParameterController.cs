using System;
using UnityEngine;

public class ParameterController : MonoBehaviour {
	private ParametersData _currentData => GameDataService.Current.parameters;
	private ProvinceManager _provinceManager;
	private TimeManager _timeManager;

	// Parámetros globales del jugador
	public float influence { get {return Mathf.Clamp(_currentData.influence, 0, 99999); } set{_currentData.influence = Mathf.Clamp(value, 0, 99999);} }
	public float fame { get {return Mathf.Clamp(_currentData.fame, -100, 100); } set{ _currentData.fame =Mathf.Clamp(value, -100, 100);} }
	public float determination { get {return Mathf.Clamp(_currentData.determination, 0, 100); } set{_currentData.determination = Mathf.Clamp(value, 0, 100);} }
	// Parámetros provinciales globales (Solo Lectura, no persistente)
	public float totalPopularity { get; private set;}
	public float totalAligned { get; private set;}
	public float totalAffiliates { get; private set;}
	public float totalPopulation {get; private set;} // Solo lectura, propositos visuales

	public Action OnParametersUpdated;
	
	void Start() {
		_timeManager = ServiceLocator.Get<TimeManager>();
		_provinceManager = ServiceLocator.Get<ProvinceManager>();

		_timeManager.OnDayPassedEvent += UpdateGlobalParameters;
		_provinceManager.OnProvincesDataLoaded += UpdateGlobalParameters;

		// Calcular población total del mapa
		foreach(var province in _provinceManager.currentMapTemplate.provinces) {
			totalPopulation += province.provincePopulation;
		}
	}

	void OnDisable() {
		_timeManager.OnDayPassedEvent -= UpdateGlobalParameters;
		_provinceManager.OnProvincesDataLoaded -= UpdateGlobalParameters;
	}

	public void UpdateGlobalParameters() {
		ComputeProvincialTotals();
		UpdateInfluence();

		OnParametersUpdated?.Invoke();
	}

	private void ComputeProvincialTotals() {
		float totalPopularity = 0;
		float totalAligned = 0;
		float totalAffiliates = 0;
		var allProvincesData = _provinceManager.GetAllGameProvinces();
		
		if (allProvincesData == null) return;
		
		foreach (var province in allProvincesData) {
			totalPopularity += province.popularity;
			totalAligned += province.aligned;
			totalAffiliates += province.affiliates;
		}

		this.totalPopularity = totalPopularity;
		this.totalAligned = totalAligned;
		this.totalAffiliates = totalAffiliates;
	}

	private void UpdateInfluence() {
		float baseInfluence = totalAffiliates / 500f;
		//? Aplicar modificadores
		_currentData.influence = baseInfluence;
	}
}