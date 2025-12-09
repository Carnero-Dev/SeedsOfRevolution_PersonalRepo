using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public event Action OnAllInstalled;

    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    public ProvinceManager provinceManager;
    public ParameterController parameterController;
    
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
        ServiceLocator.Register<ParameterController>(parameterController);
        OnAllInstalled?.Invoke();
    }
}
