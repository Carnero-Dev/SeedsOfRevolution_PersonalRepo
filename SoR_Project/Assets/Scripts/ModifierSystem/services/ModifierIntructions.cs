using UnityEngine;

[System.Serializable]
public struct ModifierInstructions  {
    [ShowOnly] public string decisionId;
    public string[] provincesToModify;
    [System.Serializable]
    public struct ParameterValue{
        public SOR_Enums.Parameters parameter;
        public float value;
        public bool isPercentege;
    }
    public ParameterValue[] parametersToModify;
    public int durationDays;
    [Tooltip("Si se deja vacío, se genera: ID_DEC_MOD_Index")]
    public string customId;  
}

