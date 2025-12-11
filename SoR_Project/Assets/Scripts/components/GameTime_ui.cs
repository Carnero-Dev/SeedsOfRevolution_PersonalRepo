using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTime_ui : MonoBehaviour {
    private TimeManager _timeManager;
    private TimeManagerData _timeData;
    [SerializeField] private TextMeshProUGUI _hourText;
    [SerializeField] private TextMeshProUGUI _dayMonthText;
    [SerializeField] private TextMeshProUGUI _yearText;
    [SerializeField] private TextMeshProUGUI _timeScaleText;
    [SerializeField] private Button _accelerateTimeButton;
    [SerializeField] private Button _decreaseTimeButton;
    [SerializeField] private Button _pauseResumeTimeButton;

    private void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();

        _accelerateTimeButton.onClick.AddListener(_timeManager.AccelerateTime);
        _decreaseTimeButton.onClick.AddListener(_timeManager.DecreaseTime);
        _pauseResumeTimeButton.onClick.AddListener(_timeManager.PauseReanudeTime);
    }

    private void Update() {
        _hourText.text = SetHourFormat();
        _dayMonthText.text = _timeData.day.ToString("00") + " de " + _timeData.month.ToString("00");
        _yearText.text = _timeData.year.ToString("0000");
        _timeScaleText.text = "x" + _timeManager.GetCurrentTimeScale().ToString();
    }

    string SetHourFormat() {
        var minute = (int)(((decimal)_timeData.hour % 1) * 100);
        minute = minute * 60 / 100;
        var hour = (int)_timeData.hour;
        return string.Format("{0:D2} : {1:D2}", hour, minute);
    }    
}
