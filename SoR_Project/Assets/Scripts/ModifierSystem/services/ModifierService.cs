using System.Collections.Generic;

public class ModifierService {
    // [ID] -> [Parámetro] -> Reporte
    private Dictionary<string, Dictionary<SOR_Enums.Parameters, ChangeReport>> _projections = new();

    public Dictionary<string, Dictionary<SOR_Enums.Parameters, ChangeReport>> Projections => _projections;

    public void GenerateProjections(IEnumerable<ActiveModifier> activeModifiers, ProvinceManager provinceManager, ParameterController parameterController) {
        _projections.Clear();

        // 1. Asegurar parámetros pasivos obligatorios (aunque no haya mods)
        EnsureMandatoryReports(parameterController);

        // 2. Cargar modificadores activos
        foreach (var mod in activeModifiers) {
            foreach (var paramMod in mod.instructions.parametersToModify) {
                if (IsGlobal(paramMod.parameter)) {
                    AddToProjection("Global", paramMod.parameter, mod.instructions.customId, paramMod.value);
                } else { 
                    if (mod.instructions.provincesToModify == null) continue;
                    foreach (var pId in mod.instructions.provincesToModify) {
                        AddToProjection(pId, paramMod.parameter, mod.instructions.customId, paramMod.value);
                    }
                }
            }
        }

        // 3. Resolver fórmulas (aplicará crecimientos pasivos sobre los reportes existentes)
        ResolveFormulas(provinceManager, parameterController);
    }

    private void EnsureMandatoryReports(ParameterController pc) {
        // Forzamos la creación del reporte de Influencia Global
        GetOrCreateReport("Global", SOR_Enums.Parameters.Influence);
		GetOrCreateReport("Global", SOR_Enums.Parameters.Fame);
		GetOrCreateReport("Global", SOR_Enums.Parameters.Determination);
    }

    private bool IsGlobal(SOR_Enums.Parameters param) {
        return param == SOR_Enums.Parameters.Influence || 
               param == SOR_Enums.Parameters.Fame || 
               param == SOR_Enums.Parameters.Determination;
    }

	private ChangeReport GetOrCreateReport(string targetId, SOR_Enums.Parameters param) {
        if (!_projections.ContainsKey(targetId)) _projections[targetId] = new();
        if (!_projections[targetId].ContainsKey(param)) _projections[targetId][param] = new();
        return _projections[targetId][param];
    }

    private void AddToProjection(string targetId, SOR_Enums.Parameters param, string source, float value) {
        if (!_projections.ContainsKey(targetId)) _projections[targetId] = new();
        if (!_projections[targetId].ContainsKey(param)) _projections[targetId][param] = new();

        var report = _projections[targetId][param];
        report.changes.Add(new ParameterChange { sourceName = source, value = value });
        report.totalBase += value;
    }

    private void ResolveFormulas(ProvinceManager provinceManager, ParameterController parameterController) {
        foreach (var targetEntry in _projections) {
            foreach (var paramEntry in targetEntry.Value) {
				var param = paramEntry.Key;
                var report = paramEntry.Value;

                //? FÓRMULA: POPULARIDAD (Solo si hay base positiva de modificadores)
                if (param == SOR_Enums.Parameters.Popularity && targetEntry.Key != "Global") {
                    var province = provinceManager.GetProvinceById(targetEntry.Key);
                    if (province != null && report.totalBase > 0) {
                        float bonus = report.totalBase * 100f / province.Population;
                        report.finalValue = report.totalBase * bonus;
                        report.changes.Add(new ParameterChange { sourceName = "Efecto Red", value = bonus });
                        continue;
                    }
                }

				//? FÓRMULA: INFLUENCIA GLOBAL (Siempre se aplica crecimiento pasivo)
				if(param == SOR_Enums.Parameters.Influence && targetEntry.Key == "Global") {
					float pasiveGrowth = parameterController.totalAffiliates / 500f;
					report.finalValue = report.totalBase + pasiveGrowth;
					report.changes.Add(new ParameterChange { sourceName = "Crecimiento Pasivo", value = pasiveGrowth });
					continue;
				}
                
                report.finalValue = report.totalBase;
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