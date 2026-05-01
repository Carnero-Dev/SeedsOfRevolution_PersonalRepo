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
        var allIds = allProvinces.Select(p => p.ProvinceId).ToList();
        var presentIds = allProvinces.Where(p => p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();
        var absentIds = allProvinces.Where(p => !p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();

        //? CASO 1: Array vacío -> Warning y omitir parámetros provinciales
        if (instructions.provincesToModify == null || instructions.provincesToModify.Length == 0) {
            if (HasProvincialParams(this.instructions)) {
                Debug.LogWarning($"[ActiveModifier] {instructions.customId} tiene parámetros provinciales pero el array de provincias está VACÍO. Los efectos provinciales se ignorarán.");
                // Opcional: Podrías filtrar los parámetros aquí para dejar solo los Globales
            }
            this.instructions.provincesToModify = new string[0];
        }
        //? CASO 2: Flags específicas
        else {
            this.instructions.provincesToModify = ProcessFlags(this.instructions, presentIds, absentIds, allIds);
        }

        this.expirationDate = CalculateExpirationDate(this.instructions.durationDays, timeManager);
        Debug.Log($"Modifier {this.instructions.customId} iniciado en {this.instructions.provincesToModify.Length} provincias");
    }
    private string[] ProcessFlags(ModifierInstructions inst, List<string> presentCandidates, List<string> absentCandidates, List<string> allIds) {
        List<string> rawList = inst.provincesToModify.Select(s => s.ToLower()).ToList();

        // 1. Flags Globales (Prioridad)
        if (rawList.Contains("[all]") || rawList.Contains("[all_fragmented]")) {
            if (rawList.Contains("[all_fragmented]")) DivideProvincialValues(inst, allIds.Count);
            return allIds.ToArray();
        }

        // 2. Flags de Grupo
        if (rawList.Contains("[all_player_here]") || rawList.Contains("[all_player_here_fragmented]")) {
            if (rawList.Contains("[all_player_here_fragmented]")) DivideProvincialValues(inst, presentCandidates.Count);
            return presentCandidates.ToArray();
        }
        
        if (rawList.Contains("[all_player_away]") || rawList.Contains("[all_player_away_fragmented]")) {
            if (rawList.Contains("[all_player_away_fragmented]")) DivideProvincialValues(inst, absentCandidates.Count);
            return absentCandidates.ToArray();
        }

        // 3. Flags Unitarias e IDs específicos
        // Filtramos: Solo dejamos IDs que existen en el mundo y NO sean flags (que no empiecen por "[")
        List<string> finalIds = inst.provincesToModify
            .Where(s => !s.StartsWith("[") && allIds.Contains(s))
            .ToList();

        List<string> flags = inst.provincesToModify.Where(s => s.StartsWith("[")).ToList();

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
            }

            if (selectedId != null) finalIds.Add(selectedId);
        }

        return finalIds.ToArray(); // Devolvemos la lista limpia de strings "["
    }

    private void DivideProvincialValues(ModifierInstructions inst, float divisor) {
        // Si el divisor es 0 (ej: all_player_here y no estás en ninguna provincia) o 1, no dividimos
        if (divisor <= 1) return; 
        
        for (int i = 0; i < inst.parametersToModify.Length; i++) {
            if (IsProvincial(inst.parametersToModify[i].parameter)) {
                inst.parametersToModify[i].value /= divisor;
            }
        }
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