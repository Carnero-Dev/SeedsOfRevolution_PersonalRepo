using System;
using UnityEngine;

[CreateAssetMenu(fileName = "[provinceId]", menuName = "SOR/Province")]
public class SO_Province : ScriptableObject
{
    [ShowOnly] public string provinceId;
    [ShowOnly] public string provinceName;
    [ShowOnly] public int provincePopulation;   
    public int provinceStability;   
    [ShowOnly] public string provinceType = SOR_Enums.provinceType.ToString();
}
