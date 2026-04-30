using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ActiveModifier {
    public ModifierInstructions instructions;
    public ExpirationDate expirationDate;

	public void Initialize(ModifierInstructions instructions, ProvinceManager provinceManager, TimeManager timeManager) {
        this.instructions = instructions;
        this.instructions.parametersToModify = instructions.parametersToModify
            .Select(p => new SOR_Enums.ParameterValue { 
                parameter = p.parameter,
                value = p.value
            }).ToArray();
        var allProvinces = provinceManager.GetAllGameProvinces(); 
        var presentIds = allProvinces.Where(p => p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();
        var absentIds = allProvinces.Where(p => !p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();

        //? CASO 1: Array vacío -> Repartir valor entre provincias donde está el jugador
        if (instructions.provincesToModify == null || instructions.provincesToModify.Length == 0) {
            if (HasProvincialParams(this.instructions) && presentIds.Count > 0) {
                float divisor = presentIds.Count;
                for (int i = 0; i < this.instructions.parametersToModify.Length; i++) {
                    if (IsProvincial(this.instructions.parametersToModify[i].parameter)) {
                        this.instructions.parametersToModify[i].value /= divisor;
                    }
                }
                this.instructions.provincesToModify = presentIds.ToArray();
            }
        } 
        //? CASO 2: Flags específicas
        else {
            ProcessFlags(this.instructions, presentIds, absentIds);
        }

        this.expirationDate = CalculateExpirationDate(this.instructions.durationDays, timeManager);
        Debug.Log($"Modifier {this.instructions.customId} iniciado en {this.instructions.provincesToModify.Length} provincias");
    }
    private void ProcessFlags(ModifierInstructions inst, List<string> presentCandidates, List<string> absentCandidates) {
        // 1. Solo IDs que realmente existan en el mundo (evita IDs basura en el array)
        var allProvinceIds = ServiceLocator.Get<ProvinceManager>().GetAllGameProvinces().Select(p => p.ProvinceId);
        List<string> finalIds = inst.provincesToModify
            .Where(s => !s.StartsWith("[") && allProvinceIds.Contains(s))
            .ToList();

        List<string> flags = inst.provincesToModify.Where(s => s.StartsWith("[")).ToList();

        // Limpiar candidatos
        presentCandidates.RemoveAll(id => finalIds.Contains(id));
        absentCandidates.RemoveAll(id => finalIds.Contains(id));

        foreach (string rawFlag in flags) {
            string flag = rawFlag.ToLower();
            string selectedId = null;

            if (flag == "[player_here]" && presentCandidates.Count > 0) {
                selectedId = presentCandidates[SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, presentCandidates.Count)];
                presentCandidates.Remove(selectedId);
            } 
            else if (flag == "[player_away]" && absentCandidates.Count > 0) {
                selectedId = absentCandidates[SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, absentCandidates.Count)];
                absentCandidates.Remove(selectedId);
            } else Debug.LogError($"Flag {flag} not recognized");

            if (selectedId != null) finalIds.Add(selectedId);
        }
        inst.provincesToModify = finalIds.ToArray();
        this.instructions = inst;
    }

    private bool HasProvincialParams(ModifierInstructions inst) => 
        inst.parametersToModify.Any(p => IsProvincial(p.parameter));

    private bool IsProvincial(SOR_Enums.Parameters p) => 
        p != SOR_Enums.Parameters.Influence && p != SOR_Enums.Parameters.Fame && p != SOR_Enums.Parameters.Determination;
        
    ExpirationDate CalculateExpirationDate(int durationDays, TimeManager timeManager) {
        // Si es permanente (-1), usamos el valor máximo de int
        if (durationDays <= -1) return new ExpirationDate(int.MaxValue);

        // Día actual + duración = Día de muerte
        int targetDay = timeManager.CurrentAbsDay + durationDays;
        return new ExpirationDate(targetDay);
    }
}

[Serializable]
public struct ExpirationDate {
    public int absoluteDay; // Día relativo al inicio del juego

    public ExpirationDate(int absDay) {
        absoluteDay = absDay;
    }
}