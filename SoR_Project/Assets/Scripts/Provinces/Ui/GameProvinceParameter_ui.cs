using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameProvinceParameter_ui : MonoBehaviour {
    private ProvinceManager _provinceManager;
    private TimeManager _timeManager;
    
    [SerializeField] private Transform provinceParameterUI;
    [SerializeField] private TextMeshProUGUI provinceNameText;
    [SerializeField] private Image provinceFlagImage;

    [SerializeField] private TextMeshProUGUI provinceStabilityText;
    [SerializeField] private TextMeshProUGUI provincePopulationText;
    [SerializeField] private TextMeshProUGUI provincePopularityText;
    [SerializeField] private TextMeshProUGUI provinceAlignedText;
    [SerializeField] private TextMeshProUGUI provinceAffiliatesText;

    private void Start() {
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager = ServiceLocator.Get<TimeManager>();
        
        _provinceManager.OnProvinceSelected += ProvinceInfoVisibility; 

        // Ruta en carpeta REsource de la bandera que queremos mostrar

        ProvinceInfoVisibility(false);
    }

    private void Update() {
        if(_provinceManager.selectedProvince == null) return;
        UpdateUi();
    }

	private void OnDisable() {
        _provinceManager.OnProvinceSelected -= ProvinceInfoVisibility;
	}
	private void UpdateUi() {
        if(_provinceManager.selectedProvince == null) return;
        provinceNameText.text = _provinceManager.selectedProvince.ProvinceName;
            provinceFlagImage.sprite = Resources.Load<Sprite>("ProvinceFlags/default/" + "flag_"+_provinceManager.selectedProvince.GetProvinceId());
        provinceStabilityText.text = "Estabilidad: " + _provinceManager.selectedProvince.stability.ToString("F2") + "%";
        provincePopulationText.text = "Habitantes: " + _provinceManager.selectedProvince.Population.ToString("N0");
        provincePopularityText.text = _provinceManager.selectedProvince.popularity.ToString("N0");
        provinceAlignedText.text = _provinceManager.selectedProvince.aligned.ToString("N0");
        provinceAffiliatesText.text = _provinceManager.selectedProvince.affiliates.ToString("N0");
    }

    private void ProvinceInfoVisibility(bool show) {
        provinceParameterUI.gameObject.SetActive(show);
        if(show) UpdateUi();
    }

}
