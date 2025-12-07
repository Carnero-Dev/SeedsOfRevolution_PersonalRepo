using System;
using System.Collections.Generic;
[Serializable]
public class MetaData : IData {
    public string profileName = "unasigned";
    public int highScore = 0;
    public List<ArchivementData> archivements;
}
