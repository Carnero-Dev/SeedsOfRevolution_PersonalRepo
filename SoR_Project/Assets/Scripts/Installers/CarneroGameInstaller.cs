using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public event Action OnAllInstalled;
    public bool IsMainMenu = false;


    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    private void Awake() {
        if(IsMainMenu) InstallMainMenu();
        else InstallGameScene(); 
    }

	void OnEnable() {
        OnAllInstalled += gameManager.StartGame;
	}
    void OnDisable() {
        OnAllInstalled -= gameManager.StartGame;
	}

	public void InstallGameScene() {
        ServiceLocator.Reset();
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        OnAllInstalled?.Invoke();
    }

    public void InstallMainMenu() {
		ServiceLocator.Reset();
	}
}
