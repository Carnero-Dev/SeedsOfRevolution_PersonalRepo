using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dev_InfoHud : MonoBehaviour
{
    private int _currentSeed => GameDataService.Current.run.seed;
    private TimeManager _timeManager;
    private GameInitializer _gameInitializer;
    [SerializeField] TextMeshProUGUI seedText;
    [SerializeField] TextMeshProUGUI devInfoText;
    [SerializeField] Button controlsButton;
    [SerializeField] Button closeControlsButton;
    [SerializeField] Button reanudeButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject PauseMenuRef;
    [SerializeField] GameObject ControlLayerRef;
    
    void Start(){
       _timeManager = ServiceLocator.Get<TimeManager>();
       _gameInitializer = ServiceLocator.Get<GameInitializer>();

       _gameInitializer.OnGameInitialized += InitializeData;

       controlsButton.onClick.AddListener(() => {
           ControlLayerRef.SetActive(true);
           PauseMenuRef.SetActive(false);
           _timeManager.PauseReanudeTime(true);
       });
       closeControlsButton.onClick.AddListener(() => {
           ControlLayerRef.SetActive(false);
           _timeManager.PauseReanudeTime(false);
       });
        reanudeButton.onClick.AddListener(() => {
              PauseMenuRef.SetActive(false);
              _timeManager.PauseReanudeTime(false);
        });

    }

    private void InitializeData() {
        seedText.text = "Seed: " + _currentSeed.ToString();
        devInfoText.text = "Dev Build version: " + Application.version + "\nBuild number: " + Application.buildGUID;
        PauseMenuRef.SetActive(false);
        ControlLayerRef.SetActive(false);
    }

    void OnDisable() {
        _gameInitializer.OnGameInitialized -= InitializeData;
    }


}
