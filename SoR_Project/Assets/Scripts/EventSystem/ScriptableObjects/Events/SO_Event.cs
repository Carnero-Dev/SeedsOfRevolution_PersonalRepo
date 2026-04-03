using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EVT_[CATEGORY]_[REDUCED_NAME]", menuName = "SOR/EventSystem/Event")]
public class SO_Event : ScriptableObject {
	public SOR_Enums.EventStorage eventStorage = new SOR_Enums.EventStorage();
}
