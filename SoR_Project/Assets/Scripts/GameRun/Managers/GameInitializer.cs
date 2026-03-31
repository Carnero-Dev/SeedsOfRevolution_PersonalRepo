using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private ProvinceManager _provinceManager;
    private EventManager _eventManager;
    [SerializeField] public SO_MapTemplate currentMap;
    public bool IsInitialized { get; private set; }
    public Action OnGameInitialized;

	public void Initialize() {
        Debug.Log("Initializing Game...");
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _eventManager = ServiceLocator.Get<EventManager>();
        MapDynamicInitializer.Initialize(currentMap);
        _provinceManager.Init(currentMap);
        _eventManager.Init(currentMap);
        IsInitialized = true;
        OnGameInitialized?.Invoke();
    }
}
