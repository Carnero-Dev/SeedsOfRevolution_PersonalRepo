using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ModifierManager : MonoBehaviour {
    private GameData data => GameDataService.Current;

    // Caché
    private Dictionary<string, ActiveModifier> _activeModifiers = new();  
    private TimeManager _timeManager;
    private GameInitializer _gameInitializer;

	private void Start() {
        _gameInitializer = ServiceLocator.Get<GameInitializer>();
        if (_gameInitializer.IsInitialized) {
            Init();
        } else {
            _gameInitializer.OnGameInitialized += Init;
        }
	}

	public void Init() {
        _timeManager = ServiceLocator.Get<TimeManager>();
        _timeManager.OnDayPassedEvent += CheckModifiers;
        SaveSystem.OnCallSave += SyncToData; // Guardar Datos

        _activeModifiers.Clear();
        foreach (var mod in data.activeModifiers) {
            _activeModifiers.TryAdd(mod.instructions.customId, mod);
        }
    }

	public void OnDisable() {
		_timeManager.OnDayPassedEvent -= CheckModifiers;
        SaveSystem.OnCallSave -= SyncToData;
        _gameInitializer.OnGameInitialized -= Init;
	}

	// Se asegura de que los los modificadores activos se gaurden en Json
	private void SyncToData() {
        data.activeModifiers = _activeModifiers.Values.ToListPooled();
        Debug.Log("Modifier Synced");
    } 

	public void ReadModifier(ModifierInstructions instructions) {
        if (instructions.parametersToModify == null) return;
        string id = instructions.customId;

        if (_activeModifiers.TryGetValue(id, out ActiveModifier existing)) {
            existing.Initialize(instructions);
            Debug.Log($"Modifier with id {id} Updated");
        } else {
            ActiveModifier newMod = new ActiveModifier();
            newMod.Initialize(instructions);
            _activeModifiers.Add(id, newMod);
            Debug.Log($"Modifier with id {id} Created");
        }
    }

    private void CheckModifiers() {
        CheckModifiersExpiration();
    }

    /// <summary>
    /// Check if any modifiers on dictionary are expired and remove them
    /// </summary>
    private void CheckModifiersExpiration() {
        List<string> expiredModifiers = new List<string>();
        TimeManagerData currentDate = data.gameTime;

        foreach (var kvp in _activeModifiers) {
            ActiveModifier mod = kvp.Value;
            if (IsExpired(mod.expirationDate, currentDate)) {
                expiredModifiers.Add(kvp.Key);
            }
        }

        foreach (string id in expiredModifiers) {
            _activeModifiers.Remove(id);
        }
    }

    /// <summary>
    /// Check if a modifier is expired
    /// </summary>
    private bool IsExpired(ExpirationDate expiration, TimeManagerData current) {
        if (expiration.year < current.year) return true;
        if (expiration.year == current.year && expiration.month < current.month) return true;
        if (expiration.year == current.year && expiration.month == current.month && expiration.day < current.day) return true;
        return false;
    }

}
