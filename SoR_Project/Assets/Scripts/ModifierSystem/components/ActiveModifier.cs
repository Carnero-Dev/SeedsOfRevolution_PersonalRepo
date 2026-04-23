using UnityEngine;
using System;
using UnityEditor.Compilation;
using System.Linq;
//TODO: Refactorizar timeManager para usar TimeData y asignar geters y seters

public class ActiveModifier {
    public ModifierInstructions instructions;
    public ExpirationDate expirationDate;

	public void Initialize(ModifierInstructions instructions, ProvinceManager provinceManager) {
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
        this.expirationDate = CalculateExpirationDate(instructions.durationDays);
        Debug.Log($"Modifier with id {instructions.customId} Init");
    }
    ExpirationDate CalculateExpirationDate(int durationDays) {
        TimeManagerData currentDate = GameDataService.Current.gameTime;
        DateTime date = new DateTime(currentDate.year, currentDate.month, currentDate.day);
        date = date.AddDays(durationDays);
        if (durationDays <= -1) return new ExpirationDate(0, 0, 9999); // Si es permanente, Fecha "infinita"
        return new ExpirationDate(date.Day, date.Month, date.Year);
    }
}

[Serializable]
public struct ExpirationDate {
    public int day;
    public int month;
    public int year;

    public ExpirationDate(int d, int m, int y) {
        day = d; month = m; year = y;
    }
}