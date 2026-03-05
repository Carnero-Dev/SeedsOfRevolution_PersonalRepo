using System;
using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{

    // SERVICES
    public TimeManager timeManager;
    public GameManager gameManager;
    public ProvinceManager provinceManager;
    public ParameterController parameterController;
    public GameInitializer gameInitializer;
    
    private void Awake() {
        InstallGameScene(); 
        gameManager.StartGame();
    }

    private void Start() {
    }

	public void InstallGameScene() {
        ServiceLocator.Reset();
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        ServiceLocator.Register<ProvinceManager>(provinceManager);
        ServiceLocator.Register<ParameterController>(parameterController);
        ServiceLocator.Register<GameInitializer>(gameInitializer);
    }
}
