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
    [SerializeField] TimeManager timeManager;
    [SerializeField] TextMeshProUGUI txtDate;
    [SerializeField] TextMeshProUGUI txtVelocity;
    [SerializeField] Button pauseButton;
    [SerializeField] Button reanudeButton;
    [Header("Province Info")]
    [HideInInspector] public ProvinceInfo selectedProvince;
    [SerializeField] TextMeshProUGUI txtName;
    [SerializeField] TextMeshProUGUI txtPopulation;
    [SerializeField] TextMeshProUGUI txtPopularity;
    [SerializeField] TextMeshProUGUI txtAffiliates;
    [SerializeField] TextMeshProUGUI txtDetermination;

    void Update()
    {
        SetHourFormat();
        txtDate.text = SetHourFormat() + " / " 
            + timeManager.day.ToString("D2") + " / " 
            + timeManager.month.ToString("D2") + " / " 
            + timeManager.year.ToString("D4");
        txtVelocity.text = "X" + timeManager.currentTimeScaleIndex;
        if(selectedProvince != null) {
            txtName.text = selectedProvince.ProvinceName;
            txtPopulation.text = selectedProvince.Population.ToString();
            txtPopularity.text = (selectedProvince.popularity / selectedProvince.Population * 100).ToString("F2") + " %";
            txtAffiliates.text = selectedProvince.affiliates.ToString();
            txtDetermination.text = selectedProvince.determination.ToString() + " %";
        }

        if (timeManager.currentTimeScaleIndex == 0) {
            pauseButton.gameObject.SetActive(true);
            reanudeButton.gameObject.SetActive(false);
        } else {
            pauseButton.gameObject.SetActive(false);
            reanudeButton.gameObject.SetActive(true);
        }

    }

    string SetHourFormat() {
        var minute = (int)(((decimal)timeManager.hour % 1) * 100);
        minute = minute * 60 / 100;
        var hour = (int)timeManager.hour;
        return string.Format("{0:D2} : {1:D2}", hour, minute);
    }

    public void SelectProvince(ProvinceInfo province) {
        selectedProvince = province;
    }
    
}
