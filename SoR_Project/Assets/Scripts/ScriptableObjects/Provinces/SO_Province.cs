using System;
using UnityEngine;

[CreateAssetMenu(fileName = "[provinceId]", menuName = "SOR/Province")]
public class SO_Province : ScriptableObject
{
    [ShowOnly] public string _provinceId;
    [ShowOnly] public string _name;
    [ShowOnly] public int _population;   
    [ShowOnly] public int _stability;   
    [ShowOnly] public string _provinceType = SOR_Enums.provinceType.ToString();
}
