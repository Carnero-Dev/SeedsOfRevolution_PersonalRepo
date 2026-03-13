using UnityEngine;

[System.Serializable]
public struct ModifierInstructions  {
    [ShowOnly] public string decisionId;
    public string[] provincesToModify;
    public SOR_Enums.ParameterValue[] parametersToModify;
    public int durationDays;
    [Tooltip("Si se deja vacío, se genera: ID_DEC_MOD_Index")]
    public string customId;  
}

