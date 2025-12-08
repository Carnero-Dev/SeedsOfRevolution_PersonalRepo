using UnityEngine;

public interface IGameManager
{

    void StartGame();

    void HandleGameLoaded();
    void HandleGameSaved();
    void HandleDataCleared();
}
