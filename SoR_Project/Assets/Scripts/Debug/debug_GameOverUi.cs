using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class debug_GameOverUi : MonoBehaviour
{   
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image _backgroundImage; 
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI daysText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Button backMainMenuButton;
    [SerializeField] private Button loadGameButton;

    TimeManager _timeManager;
    ParameterController _parameterController;

	public void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();
        _parameterController = ServiceLocator.Get<ParameterController>();
		loadGameButton.onClick.AddListener(() => {
            Time.timeScale = 1;
            SceneManager.LoadScene("Game_debug");
        });
        backMainMenuButton.onClick.AddListener(() => {
            Time.timeScale = 1;
            SceneManager.LoadScene("debug_MainMenu");
        });
	}

	public void GameLostScreen() {
        gameOverText.text = "Perdiste el apoyo de la población, el grupo se disuelve. Perdiste!";
        gameOverPanel.SetActive(true);
        _backgroundImage.color = new Color(1, 0, 0, 1); 
        daysText.text = "Days: " + CalculateDaysPassed();
        pointsText.text = "Points: " + CalculatePoints();
    }

    public void GameWonScreen() {
        gameOverText.text = "Convenciste al 50% de la población, ganaste!";
        gameOverPanel.SetActive(true);
        _backgroundImage.color = new Color(0, 1, 0, 1);
        daysText.text = "Days: " + CalculateDaysPassed();
        pointsText.text = "Points: " + CalculatePoints();
    }

    private string CalculateDaysPassed() {
        return _timeManager.CurrentAbsDay.ToString();
    }

    private string CalculatePoints() {
        float totalPoints;
        totalPoints = _parameterController.totalAffiliates + _parameterController.totalAligned;
        totalPoints *= _parameterController.totalPopularity / _parameterController.totalPopulation;
        totalPoints *= _parameterController.influence / 100;
        return totalPoints.ToString("N0");
    }


}
