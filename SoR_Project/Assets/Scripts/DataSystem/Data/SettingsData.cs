using System;

[Serializable]
public class SettingsData : IData {  
    public bool hasChanged;
    public VolumeData volumeData;
}
