using Unity.VisualScripting;
using UnityEngine;

public class DynamicRangeAttribute : PropertyAttribute
{
    public string maxVariableName;
    public int minValue;

    /// <summary>
    /// Aplica un Range dinámico a una variable declarando los nombres de las variables declaradas en el Script
    /// </summary>
    /// <param name="minValue">mínimo en forma de INT/param>
    /// <param name="maxVariableName">nombre de la variable máxima</param>
    public DynamicRangeAttribute(int minValue, string maxVariableName) {
        this.maxVariableName = maxVariableName;
        this.minValue = minValue;
    }
    
}
