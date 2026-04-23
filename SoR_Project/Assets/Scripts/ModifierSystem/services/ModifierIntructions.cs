using UnityEngine;

[System.Serializable]
public struct ModifierInstructions  {
    [ShowOnly] public string decisionId;
    public string[] provincesToModify;
    public SOR_Enums.ParameterValue[] parametersToModify;
    public int durationDays;
    [Tooltip("Si se deja vacío, se genera: {decisionId}_MOD_Index")]
    public string customId;  

    public ModifierInstructions Clone()
    {
        return new ModifierInstructions {
            decisionId = decisionId,
            provincesToModify = provincesToModify,
            parametersToModify = parametersToModify,
            durationDays = durationDays,
            customId = customId
        };
    }
}
