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
    public ModifierManager modificatorManager;
    public EventManager eventManager;
    public PlayerController playerController;
    
    private void Awake() {
        InstallGameScene(); 
    }

    private void Start() {
        gameManager.StartGame();
    }

	public void InstallGameScene() {
        ServiceLocator.Reset();
        ServiceLocator.Register<TimeManager>(timeManager);
        ServiceLocator.Register<IGameManager>(gameManager);
        ServiceLocator.Register<ProvinceManager>(provinceManager);
        ServiceLocator.Register<ParameterController>(parameterController);
        ServiceLocator.Register<GameInitializer>(gameInitializer);
        ServiceLocator.Register<ModifierManager>(modificatorManager);
        ServiceLocator.Register<EventManager>(eventManager);
        ServiceLocator.Register<PlayerController>(playerController);
    }
}
