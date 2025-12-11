using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;

/// <summary>
/// Script para hacer tests de parámetros y otras funciones que deben mostrarse en el HUD
/// Author: Carlos Carnero Cabrera
/// </summary>
public class debug_ParametersTest_UI : MonoBehaviour
{    
    TimeManager _timeManager;
    ProvinceManager _provinceManager;
    ParameterController _parameterController;
    private TimeManagerData _timeData => GameDataService.Current.timeData;
    [Header("Time Manager")]
    [SerializeField] TextMeshProUGUI txtDate;
    [SerializeField] TextMeshProUGUI txtVelocity;
    [SerializeField] Button pauseButton;
    [SerializeField] Button reanudeButton;
    [SerializeField] Button accelerateButton;
    [SerializeField] Button decreasesButton;
    [Header("Province Info")]
    [SerializeField] TextMeshProUGUI txtName;
    [SerializeField] TextMeshProUGUI txtPopulation;
    [SerializeField] TextMeshProUGUI txtPopularity;
    [SerializeField] TextMeshProUGUI txtAffiliates;
    [SerializeField] TextMeshProUGUI txtAligned;
    [SerializeField] TextMeshProUGUI txtStability;
    [Header("Parameters Info")]
    [SerializeField] TextMeshProUGUI txtInfluence;
    [SerializeField] TextMeshProUGUI txtTotalPopularity;
    [SerializeField] TextMeshProUGUI txtTotalAffiliates;
    [SerializeField] TextMeshProUGUI txtTotalAligned;
    [SerializeField] TextMeshProUGUI txtFame;
    [SerializeField] TextMeshProUGUI txtDetermination;

	void Start() {
		_timeManager = ServiceLocator.Get<TimeManager>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _parameterController = ServiceLocator.Get<ParameterController>();

        pauseButton.onClick.AddListener(_timeManager.PauseReanudeTime);
        reanudeButton.onClick.AddListener(_timeManager.PauseReanudeTime);
        accelerateButton.onClick.AddListener(_timeManager.AccelerateTime);
        decreasesButton.onClick.AddListener(_timeManager.DecreaseTime);
	}

	void Update() {
        string hourFormat = SetHourFormat();
        txtDate.text = hourFormat + " / " 
            + _timeData.day.ToString("D2") + " / " 
            + _timeData.month.ToString("D2") + " / " 
            + _timeData.year.ToString("D4");
        txtVelocity.text = "X" + _timeManager.GetCurrentTimeScale();
        
        if (_provinceManager.selectedProvince != null) {
            txtName.text = _provinceManager.selectedProvince.ProvinceName;
            txtPopulation.text = _provinceManager.selectedProvince.Population.ToString();
            txtPopularity.text = (_provinceManager.selectedProvince.popularity / _provinceManager.selectedProvince.Population * 100).ToString("F2") + " %";
            txtAffiliates.text = _provinceManager.selectedProvince.affiliates.ToString();
            txtAligned.text = _provinceManager.selectedProvince.aligned.ToString();
            txtStability.text = _provinceManager.selectedProvince.stability.ToString();
        }
        
        txtInfluence.text = _parameterController.influence.ToString("F2");
        txtTotalPopularity.text = (_parameterController.totalPopularity / _parameterController.totalPopulation * 100).ToString("F2") + " %";
        txtTotalAffiliates.text = _parameterController.totalAffiliates.ToString();
        txtTotalAligned.text = _parameterController.totalAligned.ToString();
        txtFame.text = _parameterController.fame.ToString();
        txtDetermination.text = _parameterController.determination.ToString();

        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility() {
        bool isPaused = _timeManager.GetCurrentTimeScale() == 0;
        pauseButton.gameObject.SetActive(isPaused);
        reanudeButton.gameObject.SetActive(!isPaused);
    }

    string SetHourFormat() {
        var minute = (int)(((decimal)_timeData.hour % 1) * 100);
        minute = minute * 60 / 100;
        var hour = (int)_timeData.hour;
        return string.Format("{0:D2} : {1:D2}", hour, minute);
    }    
}
