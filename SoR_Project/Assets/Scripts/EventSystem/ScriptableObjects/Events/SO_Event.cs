using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EVT_[CATEGORY]_[REDUCED_NAME]", menuName = "SOR/EventSystem/Event")]
public class SO_Event : ScriptableObject {
	public string eventId;
	public string title;
	public string description;
	public SOR_Enums.EventCategory category;
	public Sprite thumbnail;
	public int weight;
	public bool isUnique;
	[Tooltip("Optional, if null, use weight")] 
	public DateTime triggerDate;
	public SO_Decision[] decisions;
}
