using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ModifierManager : MonoBehaviour {
    private GameData data => GameDataService.Current;

    // Caché
    private Dictionary<string, ActiveModifier> _activeModifiers = new();  
    private GameInitializer _gameInitializer;
    private ModifierService _service = new();
    
    private ProvinceManager _provinceManager;
    private TimeManager _timeManager;

    private Dictionary<SOR_Enums.Parameters, float> _acumulatedModifierValues = new Dictionary<SOR_Enums.Parameters, float>();

    public Action OnModifiersApplied; // Evento para avisar que los modificadores ya se aplicaron (Después del tick diario)

	private void Start() {
        _gameInitializer = ServiceLocator.Get<GameInitializer>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager = ServiceLocator.Get<TimeManager>();

        if (_gameInitializer.IsInitialized) {
            Init();
        } else {
            _gameInitializer.OnGameInitialized += Init;
        }
	}

	public void Init() {
        CheckModifiers();
        SaveSystem.OnCallSave += SyncToData; // Guardar Datos
        _timeManager.OnDayPassedEvent += ApplyDailyTick;

        _activeModifiers.Clear();
        foreach (var mod in data.activeModifiers) {
            _activeModifiers.TryAdd(mod.instructions.customId, mod);
        }
        RefreshProjections();
    }

	public void OnDisable() {
        SaveSystem.OnCallSave -= SyncToData;
        _gameInitializer.OnGameInitialized -= Init;
        _timeManager.OnDayPassedEvent -= ApplyDailyTick;
	}

	// Se asegura de que los los modificadores activos se gaurden en Json
	private void SyncToData() {
        data.activeModifiers = _activeModifiers.Values.ToListPooled();
        Debug.Log("Modifier Synced");
    } 

    public void RefreshProjections() => _service.GenerateProjections(_activeModifiers.Values, _provinceManager);

    public ProvinceChangeReport GetReport(string id, SOR_Enums.Parameters p) => _service.GetReport(id, p);

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
        RefreshProjections();
        CheckModifiers();
    }
    public Dictionary<SOR_Enums.Parameters, float> GetAcumulatedModifiers() => _acumulatedModifierValues;

    public void CheckModifiers() {
        CheckModifiersExpiration();
        CalculateModifierValue();
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


    private void CalculateModifierValue() {
        _acumulatedModifierValues.Clear();
        foreach (var kvp in _activeModifiers) {
        var mod = kvp.Value;

        foreach (var paramMod in mod.instructions.parametersToModify) {
            if (!_acumulatedModifierValues.ContainsKey(paramMod.parameter)) {
                _acumulatedModifierValues.Add(paramMod.parameter, paramMod.value);
            } else {
                _acumulatedModifierValues[paramMod.parameter] += paramMod.value;
            }
        }
    }
	}
    public void ApplyDailyTick() {
    //CheckModifiers();
    //var parameterController = ServiceLocator.Get<ParameterController>();
    RefreshProjections();
        var pc = ServiceLocator.Get<ParameterController>();

        foreach (var target in _service.Projections) {
            if (target.Key == "Global") {
                foreach (var p in target.Value) ApplyGlobal(p.Key, p.Value.finalValue, pc);
            } else {
                var province = _provinceManager.GetProvinceById(target.Key);
                if (province == null) continue;
                foreach (var p in target.Value) ApplyToProvince(province, p.Key, p.Value.finalValue);
            }
        }
        
        CheckModifiersExpiration();
        RefreshProjections(); // Proyectar nuevo día
        OnModifiersApplied?.Invoke(); // Avisamos que los valores ya han cambiado

    // foreach (var kvp in _activeModifiers) {
    //     var provincesToModify = kvp.Value.instructions.provincesToModify;
    //     foreach (var paramMod in kvp.Value.instructions.parametersToModify) {
            
    //         switch (paramMod.parameter) {
    //             case SOR_Enums.Parameters.Infuelnce: 
    //                 parameterController.influence += paramMod.value;
    //                 break;
    //             case SOR_Enums.Parameters.Fame:
    //                 parameterController.fame += paramMod.value;
    //                 break;
    //             case SOR_Enums.Parameters.Determination:
    //                 parameterController.determination += paramMod.value;
    //                 break;
    //             case SOR_Enums.Parameters.Popularity:
    //                 foreach (var provinceId in provincesToModify) {
    //                     var province = _provinceManager.GetProvinceById(provinceId);
    //                     if (province != null) {
    //                         province.popularity += paramMod.value;
    //                     }
    //                 }
    //                 break;
    //             case SOR_Enums.Parameters.Aligned:
    //                 foreach (var provinceId in provincesToModify) {
    //                     var province = _provinceManager.GetProvinceById(provinceId);
    //                     if (province != null) {
    //                         province.aligned += paramMod.value;
    //                     }
    //                 }
    //                 break;
    //             case SOR_Enums.Parameters.Affiliates:
    //                 foreach (var provinceId in provincesToModify) {
    //                     var province = _provinceManager.GetProvinceById(provinceId);
    //                     if (province != null) {
    //                         province.affiliates += paramMod.value;
    //                     }
    //                 }
    //                 break; 
    //         }
    //     }
    // }
    
}
private void ApplyToProvince(ProvinceInfo p, SOR_Enums.Parameters param, float val) {
        switch (param) {
            case SOR_Enums.Parameters.Popularity: p.popularity += val; break;
            case SOR_Enums.Parameters.Aligned: p.aligned += val; break;
            case SOR_Enums.Parameters.Affiliates: p.affiliates += val; break;
        }
    }

    private void ApplyGlobal(SOR_Enums.Parameters param, float val, ParameterController pc) {
        switch (param) {
            case SOR_Enums.Parameters.Influence: pc.influence += val; break;
            case SOR_Enums.Parameters.Fame: pc.fame += val; break;
            case SOR_Enums.Parameters.Determination: pc.determination += val; break;
        }
    }

}
