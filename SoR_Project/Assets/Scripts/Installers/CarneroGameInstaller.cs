using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public event Action OnAllInstalled;
    // TODO: Handles para sistema de guardado
    public bool isActivated = false;


    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    private void Awake() {
        OnAllInstalled += StartGame;
        if(isActivated){ InstallGameScene(); }
    }

	void OnEnable()
	{
		// SaveSystem.OnGameLoaded += HandleGameLoaded;
        // SaveSystem.OnGameSaved += HandleGameSaved;
        // SaveSystem.OnGameDataCleared += HandleDataCleared;
	}
    void OnDisable()
	{
        OnAllInstalled -= StartGame;
		// SaveSystem.OnGameLoaded -= HandleGameLoaded;
        // SaveSystem.OnGameSaved -= HandleGameSaved;
        // SaveSystem.OnGameDataCleared -= HandleDataCleared;
	}

	public void InstallGameScene()
    {
        ServiceLocator.Reset();
        ServiceLocator.Register<CarneroGameInstaller>(this);
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        OnAllInstalled?.Invoke();
    }

    void StartGame()
    {
        Debug.Log("CarneroGameInstaller: Start Game.");
        // Iniciar lógica del juego
        OnAllInstalled -= StartGame;
    }

    void StartInNewGame()
    {
        // Lógica para empezar una nueva partida
    }

    void StartInLoadedGame()
    {
        // Lógica para cargar una partida guardada
    }


}
