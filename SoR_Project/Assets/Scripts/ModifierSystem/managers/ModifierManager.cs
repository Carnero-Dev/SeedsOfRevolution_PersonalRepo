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
    private ParameterController _parameterController;
    private ModifierService _service = new();
    
    private ProvinceManager _provinceManager;
    private TimeManager _timeManager;

    private Dictionary<SOR_Enums.Parameters, float> _acumulatedModifierValues = new Dictionary<SOR_Enums.Parameters, float>();

    public Action OnModifiersApplied; // Evento para avisar que los modificadores ya se aplicaron (Después del tick diario)
    public Action OnModifiersChanged; // Evento para avisar que los modificadores cambiaron (Se llama al agregar, eliminar o actualizar un modificador, pero antes de aplicar el tick diario)

	private void Start() {
        _gameInitializer = ServiceLocator.Get<GameInitializer>();
        _provinceManager = ServiceLocator.Get<ProvinceManager>();
        _timeManager = ServiceLocator.Get<TimeManager>();
        _parameterController = ServiceLocator.Get<ParameterController>();

        if (_gameInitializer.IsInitialized) {
            Init();
        } else {
            _gameInitializer.OnGameInitialized += Init;
        }
	}

	public void Init() {
        _activeModifiers.Clear();
        foreach (var mod in data.activeModifiers) {
            _activeModifiers.TryAdd(mod.instructions.customId, mod);
        }
        CheckModifiers();
        _parameterController.UpdateGlobalParameters();
        RefreshProjections();
        SaveSystem.OnCallSave += SyncToData; // Guardar Datos
        _timeManager.OnDayPassedEvent += ApplyDailyTick;

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

    // Llama al servicio para generar las proyecciones actuales y avisa que los modificadores cambiaron (para actualizar UI u otros sistemas relacionados)
    public void RefreshProjections() {
        _service.GenerateProjections(_activeModifiers.Values, _provinceManager, _parameterController);
        OnModifiersChanged?.Invoke();
    }

    // Métodos de consulta para UI u otros sistemas
    public ChangeReport GetReport(string id, SOR_Enums.Parameters p) => _service.GetReport(id, p);
    public float GetTotalProvincesParameterValue(SOR_Enums.Parameters p) => _service.GetTotalProvincesParameterValue(p);

    // Método para leer un nuevo modificador (desde eventos, decisiones, etc), lo agrega o actualiza en la lista de activos, chequea expiración y refresca proyecciones
	public void ReadModifier(ModifierInstructions instructions) {
        if (instructions.parametersToModify == null) return;
        string id = instructions.customId;
        ModifierInstructions instance = instructions.Clone();
        

        if (_activeModifiers.TryGetValue(id, out ActiveModifier existing)) {
            existing.Initialize(instance, _provinceManager);
            Debug.Log($"Modifier with id {id} Updated");
        } else {
            ActiveModifier newMod = new ActiveModifier();
            newMod.Initialize(instance, _provinceManager);
            _activeModifiers.Add(id, newMod);
            Debug.Log($"Modifier with id {id} Created");
        }
        CheckModifiers();
        RefreshProjections();
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
    private void ApplyDailyTick() {
        foreach (var target in _service.Projections) {
            if (target.Key == "Global") {
                foreach (var p in target.Value) ApplyGlobal(p.Key, p.Value.finalValue, _parameterController);
            } else {
                var province = _provinceManager.GetProvinceById(target.Key);
                if (province == null) continue;
                foreach (var p in target.Value) ApplyToProvince(province, p.Key, p.Value.finalValue);
            }
        }
        OnModifiersChanged?.Invoke(); // Avisamos que los modificadores cambiaron
        CheckModifiersExpiration();
        RefreshProjections(); // Proyectar nuevo día
        OnModifiersApplied?.Invoke(); // Avisamos que los valores ya han cambiado
    
    }
    private void ApplyToProvince(ProvinceInfo p, SOR_Enums.Parameters param, float val) {
        switch (param) {
            case SOR_Enums.Parameters.Popularity: p.popularity += val; break;
            case SOR_Enums.Parameters.Aligned: p.aligned += val; break;
            case SOR_Enums.Parameters.Affiliates: p.affiliates += val; break;
            case SOR_Enums.Parameters.Stability: p.stability += val; break;
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
