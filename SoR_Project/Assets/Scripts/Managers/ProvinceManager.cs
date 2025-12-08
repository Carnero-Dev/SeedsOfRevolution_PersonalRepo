using UnityEngine;

public class ProvinceManager : MonoBehaviour {
	public ProvinceInfo selectedProvince;

	public void SelectProvince(ProvinceInfo province) {
		if(province == null) {
			selectedProvince = null;
			return;
		} 
		selectedProvince = province;
	}
}