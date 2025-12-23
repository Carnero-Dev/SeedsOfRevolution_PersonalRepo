using UnityEngine;

[CreateAssetMenu(fileName = "MapTemplate", menuName = "SOR/MapTemplate")]
public class SO_MapTemplate : ScriptableObject {
	public string mapID;
	public string mapName;
	public SO_Province[] provinces;
}
