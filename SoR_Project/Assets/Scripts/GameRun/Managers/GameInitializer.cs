using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private ProvinceManager _provinceManager;
    private EventManager _eventManager;
    private TimeManager _timeManager;
    [SerializeField] public SO_MapTemplate currentMap;
    public bool IsInitialized { get; private set; }
    
    public Action OnGameInitialized;

	public void Initialize(bool isNewGame) {
        Debug.Log("Initializing Game...");
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _eventManager = ServiceLocator.Get<EventManager>();
        _timeManager = ServiceLocator.Get<TimeManager>();
        MapDynamicInitializer.Initialize(currentMap);
        _provinceManager.Init(currentMap);
        _eventManager.Init(currentMap);
        _timeManager.Init(currentMap.calendarConfig);
        if (isNewGame) _timeManager.SetTimeInputActive(false); // Desactivamos input de tiempo si es partida nueva
        IsInitialized = true;
        OnGameInitialized?.Invoke();
    }
}
