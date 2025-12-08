using UnityEngine;

public class ProvinceManager : MonoBehaviour {
	private GameData data => GameDataService.Current;
	public ProvinceInfo selectedProvince;
	public SO_MapTemplate currentMapTemplate;

	public void SelectProvince(ProvinceInfo province) {
		if(province == null) {
			selectedProvince = null;
			return;
		} 
		selectedProvince = province;
	}

	public void Init(SO_MapTemplate mapTemplate) {
		if (data.provinces.Length > 0) {
			// Si ya hay datos, asume que es una partida cargada.
			Debug.Log("Datos de provincia ya inicializados (Partida Cargada).");
			return;
		}

		var provinceList = new System.Collections.Generic.List<ProvinceData>();

		foreach (var province in mapTemplate.provinces) {
			var newProvince = CreateInitialProvince(province);
			provinceList.Add(newProvince);
		}
		data.provinces = provinceList.ToArray();
		Debug.Log($"{data.provinces.Length} Provincias inicializadas para nueva partida.");
	}
	
	private ProvinceData CreateInitialProvince(SO_Province soProvince) {
		return new ProvinceData {
		provinceID = soProvince._provinceId,
		stability = soProvince._stability,
		};
	}

	// El cache para el acceso en tiempo de ejecución
	private System.Collections.Generic.Dictionary<string, ProvinceData> _stateCache;

	public void LoadStateCache() {
		_stateCache = new System.Collections.Generic.Dictionary<string, ProvinceData>();

		foreach (var province in data.provinces) {
			// Usar el ID como clave
			_stateCache.Add(province.provinceID, province); 
		}
	}

	// Método de acceso que usará el resto del juego
	public ProvinceData GetProvinceState(string provinceId) {
		if (_stateCache.TryGetValue(provinceId, out var state)) {
			return state;
		}
		Debug.LogError($"Estado de provincia con ID {provinceId} no encontrado en la caché.");
		return null;
	}
}