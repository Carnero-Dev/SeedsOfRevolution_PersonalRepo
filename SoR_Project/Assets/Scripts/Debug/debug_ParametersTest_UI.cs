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
    [Header("Time Manager")]
    TimeManager _timeManager;
    ProvinceManager _provinceManager;
    private TimeManagerData _timeData => GameDataService.Current.timeManager;
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
    [SerializeField] TextMeshProUGUI txtDetermination;

	void Start() {
		_timeManager = ServiceLocator.Get<TimeManager>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();

        pauseButton.onClick.AddListener(_timeManager.ReanudeTime);
        reanudeButton.onClick.AddListener(_timeManager.PauseTime);
        accelerateButton.onClick.AddListener(_timeManager.AccelerateTime);
        decreasesButton.onClick.AddListener(_timeManager.DecreaseTime);
	}

	void Update()
    {
        SetHourFormat();
        txtDate.text = SetHourFormat() + " / " 
            + _timeData.day.ToString("D2") + " / " 
            + _timeData.month.ToString("D2") + " / " 
            + _timeData.year.ToString("D4");
        txtVelocity.text = "X" + _timeManager.currentTimeScaleIndex;
        if(_provinceManager.selectedProvince != null) {
            txtName.text = _provinceManager.selectedProvince.ProvinceName;
            txtPopulation.text = _provinceManager.selectedProvince.Population.ToString();
            txtPopularity.text = (_provinceManager.selectedProvince.popularity / _provinceManager.selectedProvince.Population * 100).ToString("F2") + " %";
            txtAffiliates.text = _provinceManager.selectedProvince.affiliates.ToString();
            txtDetermination.text = _provinceManager.selectedProvince.determination.ToString() + " %";
        }

        if (_timeManager.currentTimeScaleIndex == 0) {
            pauseButton.gameObject.SetActive(true);
            reanudeButton.gameObject.SetActive(false);
        } else {
            pauseButton.gameObject.SetActive(false);
            reanudeButton.gameObject.SetActive(true);
        }

    }

    string SetHourFormat() {
        var minute = (int)(((decimal)_timeData.hour % 1) * 100);
        minute = minute * 60 / 100;
        var hour = (int)_timeData.hour;
        return string.Format("{0:D2} : {1:D2}", hour, minute);
    }    
}
