using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public event Action OnAllInstalled;
    public bool isActivated = false;
    public bool IsMainMenu = false;


    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    private void Awake() {
        OnAllInstalled += StartGame;
        if(isActivated){ 
            if(IsMainMenu) InstallMainMenu();
            else InstallGameScene(); 
        }
    }

	void OnEnable() {
		SaveSystem.OnGameLoaded += HandleGameLoaded;
        SaveSystem.OnGameSaved += HandleGameSaved;
        SaveSystem.OnGameDataCleared += HandleDataCleared;
	}
    void OnDisable() {
        OnAllInstalled -= StartGame;
		SaveSystem.OnGameLoaded -= HandleGameLoaded;
        SaveSystem.OnGameSaved -= HandleGameSaved;
        SaveSystem.OnGameDataCleared -= HandleDataCleared;
	}

	public void InstallGameScene() {
        ServiceLocator.Reset();
        ServiceLocator.Register<CarneroGameInstaller>(this);
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        OnAllInstalled?.Invoke();
    }

    public void InstallMainMenu() {
		ServiceLocator.Reset();
        ServiceLocator.Register<CarneroGameInstaller>(this);
	}

#region Handlers
    void HandleGameLoaded() {
		Debug.Log("Game Loaded");
	}

    void HandleGameSaved() {
        Debug.Log("Game Saved");
    }

    void HandleDataCleared() {
        Debug.Log("Data Cleared");
    }
#endregion
# region GameFlow
    void StartGame() {
        OnAllInstalled -= StartGame;
        if (SaveSystem.IsFileExist<GameData>()) StartInContinueGame();
        else StartInNewGame(SeedRandom.GetDebugState()); 
        Debug.Log("Starting Game");
    }

    void StartInNewGame(bool debugEnabled) {
        // INICIALIZAR SISTEMA DE GUARDADO
        var newData = DataFactory.Create<GameData>();
        GameDataService.Init(newData);
        SeedRandom.Init(newData.run.seed);

        SaveSystem.Save(newData);
        Debug.Log($"New Game, debug mode {debugEnabled}");
    }

    void StartInContinueGame() {
        // INIZIALIZAR DATOS DE CARGA
        var data = SaveSystem.Load<GameData>();
        GameDataService.Init(data);
        SeedRandom.RestoreState(data.run.seedState);
        Debug.Log("Continue Game");
    }

#endregion
}
