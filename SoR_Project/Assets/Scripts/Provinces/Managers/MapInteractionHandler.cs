using UnityEngine;
using UnityEngine.UI;

public class MapInteractionHandler : MonoBehaviour, IInteractable {
    public Texture2D colorMap;
    private ProvinceManager _provinceManager;

    public void Setup(Texture2D texture) {
        colorMap = texture;
        _provinceManager = ServiceLocator.Get<ProvinceManager>();

    }

    void DetectProvince(RaycastHit hit) {
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= colorMap.width;
            pixelUV.y *= colorMap.height;

            Color clickedColor = colorMap.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            
            _provinceManager.SelectProvinceByColor(clickedColor);
    }

	public void LeftClickInteract(RaycastHit hitinfo) {
		DetectProvince(hitinfo);
	}

	public void OnHover(RaycastHit hitinfo) {
        //if(_provinceManager.selectedProvince == this) return;
	}

	public void OnDeselect() {
		_provinceManager.DeselectProvince();
	}

	public void OnUnhover() {   
        //if(_provinceManager.selectedProvince == this) return;
	}
}