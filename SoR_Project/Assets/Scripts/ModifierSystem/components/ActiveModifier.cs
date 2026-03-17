using UnityEngine;
using System;
using UnityEditor.Compilation;
//TODO: Refactorizar timeManager para usar TimeData y asignar geters y seters

public class ActiveModifier {
    public ModifierInstructions instructions;
    public ExpirationDate expirationDate;

	public void Initialize(ModifierInstructions instructions) {
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