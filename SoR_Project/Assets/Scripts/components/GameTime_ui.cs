using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTime_ui : MonoBehaviour {
    private TimeManager _timeManager;
    private TimeManagerData _timeData => GameDataService.Current.timeData;
    [SerializeField] private TextMeshProUGUI _hourText;
    [SerializeField] private TextMeshProUGUI _dayMonthText;
    [SerializeField] private TextMeshProUGUI _yearText;
    [SerializeField] private TextMeshProUGUI _timeScaleText;
    [SerializeField] private Button _accelerateTimeButton;
    [SerializeField] private Button _decreaseTimeButton;
    [SerializeField] private Button _pauseTimeButton;
    [SerializeField] private Button _resumeTimeButton;

    private void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();

        _accelerateTimeButton.onClick.AddListener(_timeManager.AccelerateTime);
        _decreaseTimeButton.onClick.AddListener(_timeManager.DecreaseTime);
        _pauseTimeButton.onClick.AddListener(UpdateButtonVisibility);
        _resumeTimeButton.onClick.AddListener(UpdateButtonVisibility);
    }

    private void Update() {
        _hourText.text = _timeData.hour.ToString("00") + " : 00";
        _dayMonthText.text = _timeData.day.ToString("00") + " de " + GetMonthName(_timeData.month);
        _yearText.text = _timeData.year.ToString("0000");
        _timeScaleText.text = "x" + _timeManager.GetCurrentTimeScale().ToString();
    } 

    private void UpdateButtonVisibility() {
        _timeManager.PauseReanudeTime();
        bool isPaused = _timeManager.GetCurrentTimeScale() == 0;
        _pauseTimeButton.gameObject.SetActive(isPaused);
        _resumeTimeButton.gameObject.SetActive(!isPaused);
    }

    private string GetMonthName(int month) => month switch {
        1 => "Enero",
        2 => "Febrero",
        3 => "Marzo",
        4 => "Abril",
        5 => "Mayo",
        6 => "Junio",
        7 => "Julio",
        8 => "Agosto",
        9 => "Septiembre",
        10 => "Octubre",
        11 => "Noviembre",
        12 => "Diciembre",
        _ => "Error"
    };
}
