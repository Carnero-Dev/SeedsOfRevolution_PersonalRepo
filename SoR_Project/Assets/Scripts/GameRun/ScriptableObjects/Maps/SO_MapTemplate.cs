using UnityEngine;

[CreateAssetMenu(fileName = "MapTemplate", menuName = "SOR/MapTemplate")]
public class SO_MapTemplate : ScriptableObject {
	public string mapID;
	public string mapName;
	public Texture2D colorMap;
	public SO_Province[] provinces;
	public SO_Event[] eventsBatery;
	public SO_CalendarConfig calendarConfig;
}
