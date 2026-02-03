using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public MapDynamicInitializer mapDynamicInitializer;
    private ProvinceManager _provinceManager;

    public event Action OnGameInitialized;
	void Start() {
		Initialize();
	}

	public void Initialize() {
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        mapDynamicInitializer.Initialize();
        _provinceManager.Init(mapDynamicInitializer.currentMap);
        OnGameInitialized?.Invoke();
    }
}
