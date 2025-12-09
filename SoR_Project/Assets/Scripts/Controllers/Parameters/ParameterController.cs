using UnityEngine;

public class ParameterController : MonoBehaviour {
	private ParametersData _currentData => GameDataService.Current.parameters;
	private ProvinceManager _provinceManager;
	private TimeManager _timeManager;

	// Parámetros globales del jugador
	public float influence { get {return _currentData.influence; } set{_currentData.influence = value;} }
	public float fame { get {return _currentData.fame; } set{_currentData.fame = value;} }
	public float determination { get {return _currentData.determination; } set{_currentData.determination = value;} }
	// Parámetros globales provinciales (SOLO LECTURA COMPUTADA)
	public float totalPopularity => _currentData.totalPopularity;
	public float totalAligned => _currentData.totalAligned;
	public float totalAffiliates => _currentData.totalAffiliates;
	public float totalPopulation {get; private set;} // Solo lectura, propositos visuales
	
	void Start() {
		_timeManager = ServiceLocator.Get<TimeManager>();
		_provinceManager = ServiceLocator.Get<ProvinceManager>();

		_timeManager.OnDayPassedEvent += UpdateGlobalParameters;

		// Calcular población total del mapa
		foreach(var province in _provinceManager.currentMapTemplate.provinces) {
			totalPopulation += province._population;
		}
	}

	void OnDisable() {
		_timeManager.OnDayPassedEvent -= UpdateGlobalParameters;
	}

	public void UpdateGlobalParameters() {
		ComputeProvincialTotals();
		UpdateInfluence();
	}

	private void ComputeProvincialTotals() {
		float totalPop = 0;
		float totalAlign = 0;
		float totalAffil = 0;
		var allProvincesData = _provinceManager.GetAllProvincesData();
		
		if (allProvincesData == null) return;
		
		foreach(var province in allProvincesData) {
			totalPop += province.popularity;
			totalAlign += province.aligned;
			totalAffil += province.affiliates;
		}


		_currentData.totalPopularity = totalPop;
		_currentData.totalAligned = totalAlign;
		_currentData.totalAffiliates = totalAffil;

	}

	private void UpdateInfluence() {
		float baseInfluence = _currentData.totalAffiliates / 500f;
		//? Aplicar modificadores
		_currentData.influence = baseInfluence;
	}
}