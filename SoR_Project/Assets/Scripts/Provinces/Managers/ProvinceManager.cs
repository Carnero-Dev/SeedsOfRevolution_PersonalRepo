using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;

public class ProvinceManager : MonoBehaviour {
	private GameData data => GameDataService.Current;
	public ProvinceInfo selectedProvince;
	[HideInInspector]public SO_MapTemplate currentMapTemplate;
	// El cache para el acceso en tiempo de ejecución
	private Dictionary<string, string> _colorToProvinceId;
	private Dictionary<string, ProvinceInfo> _gameProvinces;
	public Action OnProvincesDataLoaded;
	public Action<bool> OnProvinceSelected;

	public void SelectProvinceByColor(string color) {
		ProvinceInfo province = GetProvinceByColor(color);
		if(province == null) {
			selectedProvince = null;
			OnProvinceSelected?.Invoke(false);
			return;
		} 
		selectedProvince = province;
		OnProvinceSelected?.Invoke(true);
	}

	public IEnumerable<ProvinceInfo> GetAllGameProvinces() {
		EnsureCacheLoaded();
		return _gameProvinces.Values;
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

	public ProvinceInfo GetProvinceByColor(string color) {
		if (_colorToProvinceId != null && _colorToProvinceId.TryGetValue(color, out string provinceId)) {
			return GetProvinceInfo(provinceId);
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
		_gameProvinces = new Dictionary<string, ProvinceInfo>();
		GameObject provinceContainer = new GameObject("Provinces");
		foreach ( var provinceSo in currentMapTemplate.provinces) {
			GameObject pObj = new GameObject($"Province_{provinceSo.provinceId}");
        	pObj.transform.SetParent(provinceContainer.transform);
			pObj.AddComponent<ProvinceInfo>().InitProvinceData(provinceSo);
			_gameProvinces.Add(provinceSo.provinceId, pObj.GetComponent<ProvinceInfo>());
		}
		foreach (var province in data.provinces) {
			if(!_gameProvinces.TryGetValue(province.provinceID, out var info)) {
				Debug.LogWarning($"Provincia con ID {province.provinceID} no encontrada en la caché.");
				continue;
			}
			info.InsertData(province);
		}
		
		OnProvincesDataLoaded?.Invoke();
		Debug.Log($"Caché de estados de provincia cargada: {_gameProvinces.Count} entradas.");
	}

	public ProvinceInfo GetProvinceInfo(string provinceId) {
		EnsureCacheLoaded();
		if (_gameProvinces.TryGetValue(provinceId, out var state)) {
			return state;
		}
		Debug.LogError($"Estado de provincia con ID {provinceId} no encontrado en la caché.");
		return null;
	}
	
	private void EnsureCacheLoaded() {
		if (_gameProvinces == null) {
			Debug.LogError("La caché de provincias no ha sido cargada. ¿Se llamó a Init o LoadStateCache?");
		}
	}
}