using UnityEngine;

public interface IGameManager
{
    void StartGame();
    void OnLoadGame();
    void OnNewGame();

    void HandleGameLoaded();
    void HandleGameSaved();
    void HandleDataCleared();
}
