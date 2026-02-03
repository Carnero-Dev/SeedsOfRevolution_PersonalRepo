using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{

	private void OnEnable() {
        SaveSystem.OnGameLoaded += HandleGameLoaded;
        SaveSystem.OnGameSaved += HandleGameSaved;        
        SaveSystem.OnGameDataCleared += HandleDataCleared; 
    }

    private void OnDisable() {
        SaveSystem.OnGameLoaded -= HandleGameLoaded;
        SaveSystem.OnGameSaved -= HandleGameSaved;        
        SaveSystem.OnGameDataCleared -= HandleDataCleared;
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

#region GameFlow
	public void StartGame() {
		if (SaveSystem.IsFileExist<GameData>()) OnLoadGame();
        else OnNewGame(SeedRandom.GetDebugState()); 
        Debug.Log("Starting Game");
	}
	private void OnLoadGame() {
		// INIZIALIZAR DATOS DE CARGA
        var data = SaveSystem.Load<GameData>();
        GameDataService.Init(data);
        SeedRandom.RestoreState(data.run.seedState);
	}

	private void OnNewGame(bool debugEnabled) {
		// INICIALIZAR SISTEMA DE GUARDADO
        var newData = DataFactory.Create<GameData>();
        GameDataService.Init(newData);
        SeedRandom.Init(newData.run.seed);

        SaveSystem.Save(newData);
        Debug.Log($"New Game, debug mode: {debugEnabled}");
	}
#endregion
}
