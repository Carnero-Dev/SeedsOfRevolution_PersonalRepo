using System;
using NUnit.Framework.Internal.Builders;

[Serializable]
public class GameData : IData {
    public float version = 1.0f; // versión del GameData para auditoria
    public RunData run = new();
    public TimeManagerData timeData = new();
    public ProvinceData[] provinces = Array.Empty<ProvinceData>();
    public ParametersData parameters = new();
}
