using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class ProvinceManager : MonoBehaviour {
	private GameData data => GameDataService.Current;
	public ProvinceInfo selectedProvince;
	private SO_MapTemplate currentMapTemplate;
	// El cache para el acceso en tiempo de ejecución
	private Dictionary<string, string> _colorToProvinceId;
	private Dictionary<string, ProvinceInfo> _gameProvinces;
	public Action OnProvincesDataLoaded;
	public Action<bool> OnProvinceSelected;



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

	public IEnumerable<ProvinceInfo> GetAllGameProvinces() {
		EnsureCacheLoaded();
		return _gameProvinces.Values;
	}
	
	private ProvinceData CreateInitialProvince(SO_Province soProvince) {
		return new ProvinceData {
			provinceID = soProvince.provinceId,
			stability = soProvince.provinceStability,
		};
	}

	private void EnsureCacheLoaded() {
		if (_gameProvinces == null) {
			Debug.LogError("La caché de provincias no ha sido cargada. ¿Se llamó a Init o LoadStateCache?");
		}
	}

	public void LoadStateCache() {
		_gameProvinces = new Dictionary<string, ProvinceInfo>();
		_colorToProvinceId = new Dictionary<string, string>();

		GameObject provinceContainer = new GameObject("Provinces");
		foreach ( var p in currentMapTemplate.provinces) {
			GameObject pObj = new GameObject($"Province_{p.provinceId}");
        	pObj.transform.SetParent(provinceContainer.transform);
			pObj.AddComponent<ProvinceInfo>().InitProvinceData(p);
			
			_gameProvinces.Add(p.provinceId, pObj.GetComponent<ProvinceInfo>()); // Province Info Objects
            _colorToProvinceId.TryAdd(p.provinceColorHex.ToLower(), p.provinceId); // Province color
		}
		// PROVINCES DATA TO PROVINCE INFO
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

	public bool SelectProvinceByColor(Color color) {
		string colorHex = ColorUtility.ToHtmlStringRGB(color).ToLower();
		ProvinceInfo province = null;
		if (_colorToProvinceId != null && _colorToProvinceId.TryGetValue(colorHex, out string provinceId)) {
			province = GetProvinceById(provinceId);
		}
		if(province == null) {
			DeselectProvince();
			return false;
		} 
		selectedProvince = province;
		OnProvinceSelected?.Invoke(true);
		return true;
	}

	public void DeselectProvince() {
		selectedProvince = null;
		OnProvinceSelected?.Invoke(false);
	}

	public ProvinceInfo GetProvinceById(string provinceId) {
		EnsureCacheLoaded();
		if (_gameProvinces.TryGetValue(provinceId, out var state)) {
			return state;
		}
		Debug.LogError($"Estado de provincia con ID {provinceId} no encontrado en la caché.");
		return null;
	}

	public SO_MapTemplate GetMapTemplate() => currentMapTemplate;
}