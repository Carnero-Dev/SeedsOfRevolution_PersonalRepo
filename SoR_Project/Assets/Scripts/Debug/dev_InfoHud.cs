using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
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
       });
       closeControlsButton.onClick.AddListener(() => {
           ControlLayerRef.SetActive(false);
           _timeManager.PauseReanudeTime(false);
       });
        reanudeButton.onClick.AddListener(() =>  SimulateKeyP());
        saveButton.onClick.AddListener(() => {
            SimulateKeyP();
            SaveSystem.Save(GameDataService.Current);
        });
        loadButton.onClick.AddListener(() => {
            SimulateKeyP();
            SceneManager.LoadScene("Game_debug");
        });
        mainMenuButton.onClick.AddListener(() => {
            SimulateKeyP();
            SceneManager.LoadScene("debug_MainMenu");
        });
        exitButton.onClick.AddListener(() => Application.Quit());
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

    public void PauseOrResumeMenu(bool pause) {
        PauseMenuRef.SetActive(pause);
        if (pause) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void SimulateKeyP() {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        using (StateEvent.From(keyboard, out var eventPtr)) {
            keyboard.pKey.WriteValueIntoEvent(1f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }
}
