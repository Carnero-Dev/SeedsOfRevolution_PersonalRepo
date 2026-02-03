using UnityEngine;
using UnityEngine.UI;

public class MapInteractionHandler : MonoBehaviour {
    public Texture2D colorMap;
    private ProvinceManager _provinceManager;

    void Start() {
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            DetectProvince();
        }
    }

    public void Setup(Texture2D texture, System.Collections.Generic.Dictionary<Color, string> colorToProvinceId) {
        colorMap = texture;
        //_provinceManager

    }

    void DetectProvince() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            // Convertir el punto de impacto en coordenadas de textura (UV)
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= colorMap.width;
            pixelUV.y *= colorMap.height;

            Color clickedColor = colorMap.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            
            // Convertir Color a Hex para buscar en tu diccionario de provincias
            string colorHex = ColorUtility.ToHtmlStringRGB(clickedColor);
            
            Debug.Log("Has tocado el color: #" + colorHex);
            
            // Aquí llamarías a tu Manager:
            // _provinceManager.SelectProvinceByColor(colorHex);
        }
    }
}