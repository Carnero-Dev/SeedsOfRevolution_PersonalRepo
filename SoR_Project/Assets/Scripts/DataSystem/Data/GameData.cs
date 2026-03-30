using System;
using System.Collections.Generic;

[Serializable]
public class GameData : IData {
    public float version = 1.0f; // versión del GameData para auditoria
    public RunData run = new();
    public TimeManagerData gameTime = new();
    public ProvinceData[] provinces = Array.Empty<ProvinceData>();
    public ParametersData parameters = new();
    public List<ActiveModifier> activeModifiers = new List<ActiveModifier>();
    public EventData eventData;
}
