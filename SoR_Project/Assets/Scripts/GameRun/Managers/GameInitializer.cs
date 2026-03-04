using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public MapDynamicInitializer mapDynamicInitializer;
    private ProvinceManager _provinceManager;

    public event Action OnGameInitialized;

	public void Initialize() {
        Debug.Log("Initializing Game...");
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        mapDynamicInitializer.Initialize();
        _provinceManager.Init(mapDynamicInitializer.currentMap);
        OnGameInitialized?.Invoke();
    }
}
