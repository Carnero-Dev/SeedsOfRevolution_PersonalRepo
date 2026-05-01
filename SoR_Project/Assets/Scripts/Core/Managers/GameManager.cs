using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
    private GameInitializer _gameInitializer;
    private ParameterController _parameterController;
    private PlayerController _playerController;
    [SerializeField] private debug_GameOverUi _gameOverUi;
    private bool isNewGame;



	private void OnEnable() {
        SaveSystem.OnGameLoaded += HandleGameLoaded;
        SaveSystem.OnGameSaved += HandleGameSaved;        
        SaveSystem.OnGameDataCleared += HandleDataCleared; 
    }

    private void OnDisable() {
        SaveSystem.OnGameLoaded -= HandleGameLoaded;
        SaveSystem.OnGameSaved -= HandleGameSaved;        
        SaveSystem.OnGameDataCleared -= HandleDataCleared;
        _parameterController.OnGameLost -= LostFlow;
        _parameterController.OnGameWon -= WonFlow;
    }


	#region Handlers
	private void HandleDataCleared() {
        Debug.Log("SaveSystem: Data Cleared");
    }

    private void HandleGameLoaded() {
        Debug.Log("SaveSystem: Game Loaded");
    }

    private void HandleGameSaved() {
        Debug.Log("SaveSystem: Game Saved");
    }
	#endregion

	public void Start() {
        _playerController.input.ChangeGameMode(SOR_Enums.GameModes.Game);
    }

    private void LostFlow() {
        _playerController.input.ChangeGameMode(SOR_Enums.GameModes.UI);
        Time.timeScale = 0;
        _gameOverUi.GameLostScreen();
        Debug.Log("Game Lost");
    }

    private void WonFlow() {
        _playerController.input.ChangeGameMode(SOR_Enums.GameModes.UI);
        Time.timeScale = 0;
        _gameOverUi.GameWonScreen();
        Debug.Log("Game Won");
    }

	#region GameFlow
	public void StartGame() {
		_gameInitializer = ServiceLocator.Get<GameInitializer>();
        _parameterController = ServiceLocator.Get<ParameterController>();
        _playerController = ServiceLocator.Get<PlayerController>();

        _parameterController.OnGameLost += LostFlow;
        _parameterController.OnGameWon += WonFlow;

		if (SaveSystem.IsFileExist<GameData>()) OnLoadGame();
        else OnNewGame(SeedRandom.GetDebugState()); 
        _gameInitializer.Initialize(isNewGame);
        Debug.Log("Starting Game");
	}
	private void OnLoadGame() {
		// INIZIALIZAR DATOS DE CARGA
        var data = SaveSystem.Load<GameData>();
        GameDataService.Init(data);
        SeedRandom.RestoreState(data.run.seedState);
        isNewGame = false;
	}

	private void OnNewGame(bool debugEnabled) {
		// INICIALIZAR SISTEMA DE GUARDADO
        var newData = DataFactory.Create<GameData>();
        GameDataService.Init(newData);
        SeedRandom.Init(newData.run.seed);

        SaveSystem.Save(newData);
        Debug.Log($"New Game, debug mode: {debugEnabled}");
        isNewGame = true;
	}
#endregion
}
