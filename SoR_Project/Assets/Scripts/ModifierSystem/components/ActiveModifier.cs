using UnityEngine;
using System;
//TODO: Refactorizar timeManager para usar TimeData y asignar geters y seters

public class ActiveModifier : MonoBehaviour {
    public ModifierInstructions modifierInstruciton;
    public ExpirationDate expirationDate;

    public ActiveModifier(ModifierInstructions instructions, TimeManagerData currentDate) {
        this.modifierInstruciton = instructions;
        this.expirationDate = CalculateExpirationDate(instructions.durationDays, currentDate);
    }
    ExpirationDate CalculateExpirationDate(int durationDays, TimeManagerData currentDate) {
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