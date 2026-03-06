using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTime_ui : MonoBehaviour {
    private TimeManager _timeManager;
    private GameInitializer _gameInitializer;
    private TimeManagerData _timeData => GameDataService.Current.gameTime;
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
        _gameInitializer = ServiceLocator.Get<GameInitializer>();

        // Buttons
        _accelerateTimeButton.onClick.AddListener(_timeManager.AccelerateTime);
        _decreaseTimeButton.onClick.AddListener(_timeManager.DecreaseTime);
        _pauseTimeButton.onClick.AddListener(HandlePauseReanudeTime);
        _resumeTimeButton.onClick.AddListener(HandlePauseReanudeTime);

        

        // Init
        _gameInitializer.OnGameInitialized += () => {
            UpdateHourText();
            UpdateDateText();
            UpdateScaleText();
        };

        // Events
        _timeManager.OnHourPassedEvent += UpdateHourText;
        _timeManager.OnDayPassedEvent += UpdateDateText;
        _timeManager.OnGameTimeScaleChanged += UpdateScaleText;
    }

	private void OnDisable() {
		_timeManager.OnHourPassedEvent -= UpdateHourText;
        _timeManager.OnDayPassedEvent -= UpdateDateText;
        _timeManager.OnGameTimeScaleChanged -= UpdateScaleText;
        _gameInitializer.OnGameInitialized -= () => {
            UpdateHourText();
            UpdateDateText();
            UpdateScaleText();
        };
	}

	private void UpdateDateText() {
        _dayMonthText.text = _timeData.day.ToString("00") + " de " + GetMonthName(_timeData.month);
        _yearText.text = _timeData.year.ToString("0000");
    }

    private void UpdateScaleText()  { 
        _timeScaleText.text = "x" + _timeManager.GetCurrentTimeScale().ToString();
        bool isPaused = _timeManager.GetCurrentTimeScale() == 0;
        _pauseTimeButton.gameObject.SetActive(isPaused);
        _resumeTimeButton.gameObject.SetActive(!isPaused);
    } 

    private void UpdateHourText() => _hourText.text = _timeData.hour.ToString("00") + " : 00";


    private void HandlePauseReanudeTime() {
        _timeManager.PauseReanudeTime();
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
