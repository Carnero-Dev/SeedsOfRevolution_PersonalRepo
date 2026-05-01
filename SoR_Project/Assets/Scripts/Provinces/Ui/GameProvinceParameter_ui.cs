using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameProvinceParameter_ui : MonoBehaviour {
    private ProvinceManager _provinceManager;
    private TimeManager _timeManager;
    private GameInitializer _gameInitializer;
    
    [SerializeField] private Transform provinceParameterUI;
    [SerializeField] private TextMeshProUGUI provinceNameText;
    [SerializeField] private Image provinceFlagImage;
    [SerializeField] private Button startHere_button;

    [SerializeField] private TextMeshProUGUI provinceStabilityText;
    [SerializeField] private TextMeshProUGUI provincePopulationText;
    [SerializeField] private TextMeshProUGUI provincePopularityText;
    [SerializeField] private TextMeshProUGUI provinceAlignedText;
    [SerializeField] private TextMeshProUGUI provinceAffiliatesText;

    private void Start() {
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager = ServiceLocator.Get<TimeManager>();
        startHere_button.onClick.AddListener(StartHere);
        _gameInitializer = ServiceLocator.Get<GameInitializer>();
        _provinceManager.OnProvinceSelected += ProvinceInfoVisibility; 

        // Ruta en carpeta REsource de la bandera que queremos mostrar

        ProvinceInfoVisibility(false);
        if (_gameInitializer.IsInitialized) CheckStartStatus();
        else
        _gameInitializer.OnGameInitialized += CheckStartStatus;
    }

    private void OnDisable() {
        _provinceManager.OnProvinceSelected -= ProvinceInfoVisibility;
        _gameInitializer.OnGameInitialized -= CheckStartStatus;
    }

    private void CheckStartStatus() {
        ProvinceInfo[] provinces = _provinceManager.GetAllGameProvinces().ToArray();
        foreach (ProvinceInfo province in provinces) {
            province.CheckPlayerInProvince();
            if (province.isPlayerOnProvince) {
                startHere_button.gameObject.SetActive(false);
                return;
            }
        }
    }
    private void StartHere() {
        if(_provinceManager.selectedProvince == null) return;
        _provinceManager.selectedProvince.isPlayerOnProvince = true; //debug
        _timeManager.SetTimeInputActive(true);

        _provinceManager.selectedProvince.popularity += 100;
        _provinceManager.selectedProvince.aligned += 30;
        _provinceManager.selectedProvince.affiliates += 10;
    }

    private void Update() {
        if(_provinceManager.selectedProvince == null) return;
        UpdateUi();
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
