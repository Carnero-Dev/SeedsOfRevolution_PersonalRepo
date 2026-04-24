using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SOR/Calendar Config")]
public class SO_CalendarConfig : ScriptableObject {
    public MonthData[] months;
    public string startDate = "01/01/2024"; 

    public void ApplyStartDateToData(TimeManagerData data) {
        string[] parts = startDate.Split('/');
        data.day = int.Parse(parts[0]);
        data.month = int.Parse(parts[1]);
        data.year = int.Parse(parts[2]);
        data.minute = 0;
        data.hour = 0;
    }

    // Aseguramos que el offset se calcule antes de usarlo
    public int GetRelativeAbsDay(int d, int m, int y) {
        string[] parts = startDate.Split('/');
        int startOffset = DateToAbsDay(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        
        return DateToAbsDay(d, m, y) - startOffset;
    }

    public bool IsLeapYear(int year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

    public int GetDaysInMonth(int month, int year) {
        if (month < 1 || month > months.Length) return 30;
        int days = months[month - 1].days;
        if (month == 2 && IsLeapYear(year)) days++;
        return days;
    }

    private int GetDaysInYear(int year) {
        int total = 0;
        for (int i = 1; i <= months.Length; i++) total += GetDaysInMonth(i, year);
        return total;
    }

    // Cálculo absoluto desde el año 0 (para uso interno)
    private int DateToAbsDay(int day, int month, int year) {
        int totalDays = 0;
        for (int y = 0; y < year; y++) totalDays += GetDaysInYear(y);
        for (int m = 1; m < month; m++) totalDays += GetDaysInMonth(m, year);
        return totalDays + day;
    }
}
[Serializable]
public class MonthData {
	public string name;
	public int days;
}