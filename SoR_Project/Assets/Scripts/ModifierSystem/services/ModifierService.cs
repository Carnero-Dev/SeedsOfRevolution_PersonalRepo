using System.Collections.Generic;
using UnityEngine;

public class ModifierService {
    // [ID] -> [Parámetro] -> Reporte
    private Dictionary<string, Dictionary<SOR_Enums.Parameters, ChangeReport>> _projections = new();

    public Dictionary<string, Dictionary<SOR_Enums.Parameters, ChangeReport>> Projections => _projections;

    public void GenerateProjections(IEnumerable<ActiveModifier> activeModifiers, ProvinceManager provinceManager, ParameterController parameterController) {
        _projections.Clear();

        // Asegurar parámetros pasivos obligatorios (aunque no haya mods)
        InitializeMandatoryReports(provinceManager);

        // Cargar modificadores activos
        foreach (var mod in activeModifiers) {
            foreach (var paramMod in mod.instructions.parametersToModify) {
				// Parametros globales
                if (IsGlobal(paramMod.parameter)) {
                    AddToProjection("Global", paramMod.parameter, mod.instructions.customId, paramMod.value);
                } else { 
				// Parametros provinciales
					if (mod.instructions.provincesToModify == null) continue;
                    foreach (var pId in mod.instructions.provincesToModify) {
                        AddToProjection(pId, paramMod.parameter, mod.instructions.customId, paramMod.value);
                    }
                }
            }
        }
        // Resolver fórmulas (aplicará crecimientos pasivos sobre los reportes existentes)
        ResolveFormulas(provinceManager, parameterController);
    }

    // Forzamos la creación del reporte de parámetros que no dependan de modificadores
    private void InitializeMandatoryReports(ProvinceManager provinceManager) {
        // Parámetros Globales
        GetOrCreateReport("Global", SOR_Enums.Parameters.Influence);
        GetOrCreateReport("Global", SOR_Enums.Parameters.Fame);
        GetOrCreateReport("Global", SOR_Enums.Parameters.Determination);

        // Parámetros de cada provincia (para asegurar el crecimiento pasivo)
        foreach (var province in provinceManager.GetAllGameProvinces()) {
            GetOrCreateReport(province.ProvinceId, SOR_Enums.Parameters.Popularity);
        }
        Debug.Log($"_projections.Count: {_projections.Count}");
    }

    private bool IsGlobal(SOR_Enums.Parameters param) {
        return param == SOR_Enums.Parameters.Influence || 
               param == SOR_Enums.Parameters.Fame || 
               param == SOR_Enums.Parameters.Determination;
    }

	private ChangeReport GetOrCreateReport(string targetId, SOR_Enums.Parameters param) {
        if (!_projections.ContainsKey(targetId)) _projections[targetId] = new();
        if (!_projections[targetId].ContainsKey(param)) _projections[targetId][param] = new ChangeReport();
        return _projections[targetId][param];
    }

    private void AddToProjection(string targetId, SOR_Enums.Parameters param, string source, float value) {
        if (!_projections.ContainsKey(targetId)) _projections[targetId] = new();
        if (!_projections[targetId].ContainsKey(param)) _projections[targetId][param] = new();

        var report = _projections[targetId][param];
        report.changes.Add(new ParameterChange { sourceName = source, value = value });
        report.totalBase += value;
    }
	// Contiene todas las formulas para parámetros específicos dentro del juego. Se pueden aplicar utilizando los reportes generados para alterar modificadores actuales.
    private void ResolveFormulas(ProvinceManager provinceManager, ParameterController parameterController) {
        foreach (var targetEntry in _projections) {
            string currentTargetId = targetEntry.Key;
            foreach (var paramEntry in targetEntry.Value) {
				var param = paramEntry.Key;
                var report = paramEntry.Value;
                report.finalValue = report.totalBase;

                //? FÓRMULA: EFECTO RED (Crecimiento Pasivo)
                if (param == SOR_Enums.Parameters.Popularity && currentTargetId != "Global") {
                    var province = provinceManager.GetProvinceById(currentTargetId);
                    if (province != null) {
                        float networkIntensity = 0.01f; 
                        float passiveGrowth = province.popularity * networkIntensity;
                        report.finalValue += passiveGrowth;
                        
                        report.changes.Add(new ParameterChange { 
                            sourceName = "Efecto Red (Pasivo)", 
                            value = passiveGrowth 
                        });
                    }
                    continue;
                }
				//? FÓRMULA: ALINEADOS (Solo si hay modificadores)
                if (param == SOR_Enums.Parameters.Aligned && currentTargetId != "Global") {
                    var province = provinceManager.GetProvinceById(currentTargetId);
                    if (province != null && report.totalBase !=0) {
						float stabilityFactor = (50f - province.stability) / 100f;
                    	float stabilityBonus = report.totalBase * stabilityFactor;

						string label = stabilityFactor >= 0 ? "Baja Estabilidad" : "Alta Estabilidad";
                    	report.finalValue = report.totalBase + stabilityBonus;
                        report.changes.Add(new ParameterChange { sourceName = $"Bonus {label}", value = stabilityBonus });
                        continue;
                    }
                }

				//? FÓRMULA: INFLUENCIA GLOBAL (Siempre se aplica crecimiento pasivo)
				if(param == SOR_Enums.Parameters.Influence && currentTargetId == "Global") {
					float pasiveGrowth = parameterController.totalAffiliates / 10000f;
					report.finalValue = report.totalBase + pasiveGrowth;
					report.changes.Add(new ParameterChange { sourceName = "Crecimiento Pasivo", value = pasiveGrowth });
					continue;
				}
            }
        }
    }

    public ChangeReport GetReport(string targetId, SOR_Enums.Parameters param) {
        if (_projections.TryGetValue(targetId, out var pDict))
            if (pDict.TryGetValue(param, out var report)) return report;
        return null;
    }

	public float GetTotalProvincesParameterValue(SOR_Enums.Parameters param) {
		float total = 0;
		foreach (var targetEntry in _projections) {
			if (targetEntry.Key == "Global") continue; // Solo provincias
			if (targetEntry.Value.TryGetValue(param, out var report)) total += report.finalValue;
		}
		return total;
	}
}
[System.Serializable]
public class ParameterChange {
    public string sourceName; 
    public float value;
}
[System.Serializable]
public class ChangeReport {
    public List<ParameterChange> changes = new();
    public float totalBase;
    public float finalValue; 
}