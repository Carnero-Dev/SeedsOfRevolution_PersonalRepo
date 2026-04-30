using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ModifierInstructions {
    [ShowOnly] public string decisionId;
    public string[] provincesToModify;
    public SOR_Enums.ParameterValue[] parametersToModify;
    public int durationDays;
    [Tooltip("Si se deja vacío, se genera: {decisionId}_MOD_Index")]
    public string customId;  

    public ModifierInstructions Clone() {
        return new ModifierInstructions {
            decisionId = decisionId,
            provincesToModify = provincesToModify?.ToArray(), 
            parametersToModify = parametersToModify?.Select(p => new SOR_Enums.ParameterValue {
            parameter = p.parameter,
            value = p.value
        }).ToArray(),
        durationDays = durationDays,
        customId = customId
        };
    }
}
