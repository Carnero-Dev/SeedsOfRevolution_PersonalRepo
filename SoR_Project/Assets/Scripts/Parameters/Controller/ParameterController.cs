using System;
using Unity.VisualScripting;
using UnityEngine;

public class ParameterController : MonoBehaviour {
	private ParametersData _currentData => GameDataService.Current.parameters;
	private ProvinceManager _provinceManager;
	private TimeManager _timeManager;
	private GameInitializer _gameInitializer;
	private ModifierManager _modifierManager;

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
		_gameInitializer = ServiceLocator.Get<GameInitializer>();
		_modifierManager = ServiceLocator.Get<ModifierManager>();

		_modifierManager.OnModifiersApplied += UpdateGlobalParameters;
		
		if(_gameInitializer.IsInitialized) { ComputeProvincialTotals(); ComputeTotalPopulation(); }
		else _gameInitializer.OnGameInitialized += () => { ComputeTotalPopulation(); ComputeProvincialTotals(); };
	}

	void OnDisable() {
		_modifierManager.OnModifiersApplied -= UpdateGlobalParameters;
		_gameInitializer.OnGameInitialized -= () => UpdateGlobalParameters();
		_gameInitializer.OnGameInitialized -= () => ComputeTotalPopulation();
	}

	public void UpdateGlobalParameters() {
		ComputeProvincialTotals();

		OnParametersUpdated?.Invoke();
	}

	private void ComputeTotalPopulation() {
		foreach(var province in _provinceManager.GetMapTemplate().provinces) {
			totalPopulation += province.provincePopulation;
		}
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
}