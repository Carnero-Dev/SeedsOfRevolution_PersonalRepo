using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public event Action OnAllInstalled;

    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    public ProvinceManager provinceManager;
    
    private void Awake() {
        OnAllInstalled += gameManager.StartGame;
        InstallGameScene(); 
    }

    void OnDisable() {
        OnAllInstalled -= gameManager.StartGame;
	}

	public void InstallGameScene() {
        ServiceLocator.Reset();
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        ServiceLocator.Register<ProvinceManager>(provinceManager);
        OnAllInstalled?.Invoke();
    }
}
