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
        //provinceFlagImage.sprite = _provinceManager.selectedProvince.flagImage;
        provinceStabilityText.text = "Estabilidad: " + _provinceManager.selectedProvince.stability.ToString("F2") + "%";
        provincePopulationText.text = "Habitantes: " + _provinceManager.selectedProvince.Population.ToString();
        provincePopularityText.text = _provinceManager.selectedProvince.popularity.ToString();
        provinceAlignedText.text = _provinceManager.selectedProvince.aligned.ToString();
        provinceAffiliatesText.text = _provinceManager.selectedProvince.affiliates.ToString();
    }

    private void ProvinceInfoVisibility(bool show) {
        provinceParameterUI.gameObject.SetActive(show);
        if(show) UpdateUi();
    }

}
