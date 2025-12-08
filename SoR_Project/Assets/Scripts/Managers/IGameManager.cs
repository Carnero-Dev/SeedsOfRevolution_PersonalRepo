using UnityEngine;

public interface IGameManager
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
    void StartGame();
    void OnLoadGame();
    void OnNewGame();

    void HandleGameLoaded();
    void HandleGameSaved();
    void HandleDataCleared();
}
