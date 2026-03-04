using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private ProvinceManager _provinceManager;
    [SerializeField] public SO_MapTemplate currentMap;

    public event Action OnGameInitialized;

	public void Initialize() {
        Debug.Log("Initializing Game...");
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        MapDynamicInitializer.Initialize(currentMap);
        _provinceManager.Init(currentMap);
        OnGameInitialized?.Invoke();
    }
}
