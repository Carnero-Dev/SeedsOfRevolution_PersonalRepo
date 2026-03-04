using UnityEngine;
using UnityEngine.UI;

public class MapInteractionHandler : MonoBehaviour, IInteractable {
    public Texture2D colorMap;
    private ProvinceManager _provinceManager;

    public void Setup(Texture2D texture) {
        colorMap = texture;
        _provinceManager = ServiceLocator.Get<ProvinceManager>();

    }

    void DetectProvince() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            // Convertir el punto de impacto en coordenadas de textura (UV)
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= colorMap.width;
            pixelUV.y *= colorMap.height;

            Color clickedColor = colorMap.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            
            _provinceManager.SelectProvinceByColor(clickedColor);
        }
    }

	public void LeftClickInteract()
	{
		DetectProvince();
	}

	public void OnHover()
	{

	}

	public void OnDeselect()
	{

	}

	public void OnUnhover()
	{

	}
}