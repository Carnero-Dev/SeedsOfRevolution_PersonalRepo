using UnityEngine;
using System;
using UnityEditor.Compilation;
using System.Linq;
//TODO: Refactorizar timeManager para usar TimeData y asignar geters y seters

public class ActiveModifier {
    public ModifierInstructions instructions;
    public ExpirationDate expirationDate;

	public void Initialize(ModifierInstructions instructions, ProvinceManager provinceManager, TimeManager timeManager) {
        // Si no hay provincias especificadas, pero el modificador tiene parámetros provinciales
        if (instructions.provincesToModify == null || instructions.provincesToModify.Length == 0) {
            // Solo llenamos si realmente hay algún parámetro que NO sea global
            bool hasProvincialParams = instructions.parametersToModify
                .Any(p => p.parameter != SOR_Enums.Parameters.Influence && 
                        p.parameter != SOR_Enums.Parameters.Fame && 
                        p.parameter != SOR_Enums.Parameters.Determination);

            if (hasProvincialParams) {
                // Obtenemos todos los IDs de las provincias del manager
                instructions.provincesToModify = provinceManager.GetAllGameProvincesIds();
            }
        }
        this.instructions = instructions;
        this.expirationDate = CalculateExpirationDate(instructions.durationDays, timeManager);
        Debug.Log($"Modifier with id {instructions.customId} Init");
    }
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