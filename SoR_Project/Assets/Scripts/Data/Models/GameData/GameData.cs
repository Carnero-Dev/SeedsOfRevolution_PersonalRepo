using System;

[Serializable]
public class GameData : IData {
    public float version = 1.0f; // versión del GameData para auditoria
    public RunData run = new();
    public TimeManagerData timeManager = new();
    public ProvinceData[] provinces = Array.Empty<ProvinceData>();
}
