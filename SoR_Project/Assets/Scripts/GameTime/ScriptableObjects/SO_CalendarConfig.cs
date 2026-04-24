using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SOR/Calendar Config")]
public class SO_CalendarConfig : ScriptableObject {
    public MonthData[] months;

    public bool IsLeapYear(int year) {
        return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
    }

    public int GetDaysInMonth(int month, int year) {
        if (month < 1 || month > months.Length) return 30;
        int days = months[month - 1].days;
        if (month == 2 && IsLeapYear(year)) days++; // Regla bisiesto
        return days;
    }

    public int GetDaysInYear(int year) {
        int total = 0;
        for (int i = 1; i <= months.Length; i++) total += GetDaysInMonth(i, year);
        return total;
    }

    public int DateToAbsDay(int day, int month, int year) {
        int totalDays = 0;
        // Días de años anteriores
        for (int y = 0; y < year; y++) totalDays += GetDaysInYear(y);
        // Días de meses anteriores este año
        for (int m = 1; m < month; m++) totalDays += GetDaysInMonth(m, year);
        
        return totalDays + day;
    }
}
[Serializable]
public class MonthData {
	public string name;
	public int days;
}