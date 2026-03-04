using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class ProvinceManager : MonoBehaviour {
	private GameData data => GameDataService.Current;
	public ProvinceInfo selectedProvince;
	[HideInInspector]public SO_MapTemplate currentMapTemplate;
	// El cache para el acceso en tiempo de ejecución
	private Dictionary<string, ProvinceData> _stateCache;
	private Dictionary<string, string> _colorToProvinceId;
	public Action OnProvincesCacheLoaded;
	public Action<bool> OnProvinceSelected;

	public void SelectProvince(ProvinceInfo province) {
		if(province == null) {
			selectedProvince = null;
			OnProvinceSelected?.Invoke(false);
			return;
		} 
		selectedProvince = province;
		OnProvinceSelected?.Invoke(true);
	}

	public IEnumerable<ProvinceData> GetAllProvincesData() {
		EnsureCacheLoaded();
		return _stateCache.Values;
	}

	public void Init(SO_MapTemplate mapTemplate) {
		currentMapTemplate = mapTemplate;
		if (data.provinces.Length > 0) {
			Debug.Log("Datos de provincia ya inicializados (Partida Cargada).");
			LoadStateCache();
			return;
		}

		var provinceList = new List<ProvinceData>();
		foreach (var province in mapTemplate.provinces) {
			var newProvince = CreateInitialProvince(province);
			provinceList.Add(newProvince);
		}
		data.provinces = provinceList.ToArray();
		Debug.Log($"{data.provinces.Length} Provincias inicializadas para nueva partida.");
		LoadStateCache();	
	}

	public void SetProvinceColor(Dictionary<string, string> colorToProvinceId) {
		_colorToProvinceId = colorToProvinceId;
	}

	public ProvinceData GetProvinceByColor(string color) {
		if (_colorToProvinceId != null && _colorToProvinceId.TryGetValue(color, out string provinceId)) {
			return GetProvinceState(provinceId);
		}
		return null;
	}
	
	private ProvinceData CreateInitialProvince(SO_Province soProvince) {
		return new ProvinceData {
			provinceID = soProvince.provinceId,
			stability = soProvince.provinceStability,
		};
	}

	public void LoadStateCache() {
		_stateCache = new Dictionary<string, ProvinceData>();
		foreach (var province in data.provinces) {
			if (!_stateCache.TryAdd(province.provinceID, province)) {
                Debug.LogWarning($"ID de provincia duplicado encontrado: {province.provinceID}");
            }
		}
		OnProvincesCacheLoaded?.Invoke();
		Debug.Log($"Caché de estados de provincia cargada: {_stateCache.Count} entradas.");
	}

	public ProvinceData GetProvinceState(string provinceId) {
		EnsureCacheLoaded();
		if (_stateCache.TryGetValue(provinceId, out var state)) {
			return state;
		}
		Debug.LogError($"Estado de provincia con ID {provinceId} no encontrado en la caché.");
		return null;
	}
	
	private void EnsureCacheLoaded() {
		if (_stateCache == null) {
			Debug.LogError("La caché de provincias no ha sido cargada. ¿Se llamó a Init o LoadStateCache?");
		}
	}
}