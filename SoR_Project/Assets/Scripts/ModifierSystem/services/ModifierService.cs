using System.Collections.Generic;

public class ModifierService {
    // [ID] -> [Parámetro] -> Reporte
    private Dictionary<string, Dictionary<SOR_Enums.Parameters, ProvinceChangeReport>> _projections = new();

    public Dictionary<string, Dictionary<SOR_Enums.Parameters, ProvinceChangeReport>> Projections => _projections;

    public void GenerateProjections(IEnumerable<ActiveModifier> activeModifiers, ProvinceManager provinceManager) {
        _projections.Clear();

        foreach (var mod in activeModifiers) {
            foreach (var paramMod in mod.instructions.parametersToModify) {
                if (mod.instructions.provincesToModify == null || mod.instructions.provincesToModify.Length == 0) {
                    AddToProjection("Global", paramMod.parameter, mod.instructions.customId, paramMod.value);
                } else {
                    foreach (var pId in mod.instructions.provincesToModify) {
                        AddToProjection(pId, paramMod.parameter, mod.instructions.customId, paramMod.value);
                    }
                }
            }
        }
        ResolveFormulas(provinceManager);
    }

    private void AddToProjection(string targetId, SOR_Enums.Parameters param, string source, float value) {
        if (!_projections.ContainsKey(targetId)) _projections[targetId] = new();
        if (!_projections[targetId].ContainsKey(param)) _projections[targetId][param] = new();

        var report = _projections[targetId][param];
        report.changes.Add(new ParameterChange { sourceName = source, value = value });
        report.totalBase += value;
    }

    private void ResolveFormulas(ProvinceManager provinceManager) {
        foreach (var targetEntry in _projections) {
            foreach (var paramEntry in targetEntry.Value) {
                var report = paramEntry.Value;

                // --- Lógica de Fórmulas (Fácil de expandir aquí) ---
                if (paramEntry.Key == SOR_Enums.Parameters.Popularity && targetEntry.Key != "Global") {
                    var province = provinceManager.GetProvinceById(targetEntry.Key);
                    if (province != null && report.totalBase > 0) {
                        float bonus = report.totalBase * 100f / province.Population;
                        report.finalValue = report.totalBase * bonus;
                        report.changes.Add(new ParameterChange { sourceName = "Efecto Red", value = bonus });
                        continue;
                    }
                }
                
                report.finalValue = report.totalBase;
            }
        }
    }

    public ProvinceChangeReport GetReport(string targetId, SOR_Enums.Parameters param) {
        if (_projections.TryGetValue(targetId, out var pDict))
            if (pDict.TryGetValue(param, out var report)) return report;
        return null;
    }
}
[System.Serializable]
public class ParameterChange {
    public string sourceName; 
    public float value;
}
[System.Serializable]
public class ProvinceChangeReport {
    public List<ParameterChange> changes = new();
    public float totalBase;
    public float finalValue; 
}