using UnityEngine;
using UnityEngine.UI;

public class MapInteractionHandler : MonoBehaviour, IInteractable {
    public Texture2D colorMap;
    public Material politicalMapMaterial;
    private ProvinceManager _provinceManager;

    public void Init(Texture2D texture, Material material) {
        colorMap = texture;
        politicalMapMaterial = material;
        _provinceManager = ServiceLocator.Get<ProvinceManager>();

    }

    void DetectProvince(RaycastHit hit) {
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= colorMap.width;
            pixelUV.y *= colorMap.height;

            Color clickedColor = colorMap.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            
            HighlightProvince(clickedColor, _provinceManager.SelectProvinceByColor(clickedColor));
    }
    void HighlightProvince(Color clickedColor, bool isSelected) {
        if (isSelected) {
            politicalMapMaterial.SetColor("_selectedColor", clickedColor);
            politicalMapMaterial.SetInt("_isSelected", 1);
        } else {
            politicalMapMaterial.SetColor("_selectedColor", Color.clear);
            politicalMapMaterial.SetInt("_isSelected", 0);
        }
    }

	public void LeftClickInteract(RaycastHit hitinfo) {
		DetectProvince(hitinfo);
	}

	public void OnHover(RaycastHit hitinfo) {
        //if(_provinceManager.selectedProvince == this) return;
	}

	public void OnDeselect() {
		_provinceManager.DeselectProvince();
                HighlightProvince(Color.clear, false);
	}

	public void OnUnhover() {   
        //if(_provinceManager.selectedProvince == this) return;
	}
}