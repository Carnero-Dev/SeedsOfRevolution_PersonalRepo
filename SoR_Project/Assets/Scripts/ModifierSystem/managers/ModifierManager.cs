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
    public float GetTotalProvincesParameterValue(SOR_Enums.Parameters p) => _service.GetTotalProvincesParameterValue(p, _provinceManager);

    // Método para leer un nuevo modificador (desde eventos, decisiones, etc), lo agrega o actualiza en la lista de activos, chequea expiración y refresca proyecciones
	public void ReadModifier(ModifierInstructions instructions) {
        if (instructions.parametersToModify == null) return;
        ModifierInstructions instance = instructions.Clone();
        ActiveModifier newMod = new ActiveModifier();
        newMod.Initialize(instance, _provinceManager, _timeManager);
        string uniqueId = GenerateUniqueId(newMod.instructions);
        newMod.instructions.customId = uniqueId;

        if (_activeModifiers.ContainsKey(uniqueId)) {
            // Si ya existe exactamente en esas provincias, refrescamos (sobrescribimos)
            _activeModifiers[uniqueId] = newMod;
            Debug.Log($"Modifier {uniqueId} Refreshed");
        } else {
            // Si son provincias distintas, se añade como nuevo
            _activeModifiers.Add(uniqueId, newMod);
            Debug.Log($"Modifier {uniqueId} Created");
        }
        CheckModifiers();
        RefreshProjections();
    }
    public Dictionary<SOR_Enums.Parameters, float> GetAcumulatedModifiers() => _acumulatedModifierValues;

    public void CheckModifiers() {
        CheckModifiersExpiration();
        CalculateModifierValue();
    }

    public bool IsDecisionValid(SO_Decision decision) {
        // Si el diseñador no marcó el check, el evento salta siempre (comportamiento por defecto)
        if (!decision.skipIfNoAviableProvinces) return true;

        foreach (var mod in decision.modifiersArray) {
            if (!HasProvincialParams(mod)) continue;

            // Intentamos resolver qué provincias se verían afectadas
            var resolved = ResolveProvincesPreview(mod);
            
            // Si un modificador provincial no encuentra NI UNA provincia válida, 
            // invalidamos la decisión completa.
            if (resolved.Count == 0) return false; 
        }
        return true;
    }
    /// <summary>
    /// Check if any modifiers on dictionary are expired and remove them
    /// </summary>
    private void CheckModifiersExpiration() {
        List<string> expiredModifiers = new List<string>();
        
        // Obtenemos el día absoluto actual desde el TimeManager
        int today = _timeManager.CurrentAbsDay; 

        foreach (var kvp in _activeModifiers) {
            // Si el día de hoy ya es igual o mayor al de expiración, fuera.
            if (today >= kvp.Value.expirationDate.absoluteDay) {
                expiredModifiers.Add(kvp.Key);
            }
        }

        foreach (string id in expiredModifiers) {
            _activeModifiers.Remove(id);
            Debug.Log($"Modifier {id} expired and removed.");
        }
    }
    private string GenerateUniqueId(ModifierInstructions inst) {
        // Si no hay provincias (es global), nos quedamos con la ID base
        if (inst.provincesToModify == null || inst.provincesToModify.Length == 0) {
            return inst.customId; 
        }

        // Ordenamos los IDs para que "ProvA_ProvB" sea lo mismo que "ProvB_ProvA"
        var sortedProvinces = inst.provincesToModify.OrderBy(s => s);
        string provinceSuffix = string.Join("_", sortedProvinces);

        // Resultado: DECISIONID_MOD0_Madrid_Toledo
        return $"{inst.customId}_{provinceSuffix}";
    }
    
    private bool HasProvincialParams(ModifierInstructions inst) => 
        inst.parametersToModify.Any(p => IsProvincial(p.parameter));
    private bool IsProvincial(SOR_Enums.Parameters p) => 
        p != SOR_Enums.Parameters.Influence && p != SOR_Enums.Parameters.Fame && p != SOR_Enums.Parameters.Determination;

    private List<string> ResolveProvincesPreview(ModifierInstructions inst) {
        var allProvinces = _provinceManager.GetAllGameProvinces();
        var allIds = allProvinces.Select(p => p.ProvinceId).ToList();
        var presentIds = allProvinces.Where(p => p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();
        var absentIds = allProvinces.Where(p => !p.isPlayerOnProvince).Select(p => p.ProvinceId).ToList();

        // CASO A: Array vacío (Se aplica a donde esté el jugador)
        if (inst.provincesToModify == null || inst.provincesToModify.Length == 0) {
            return presentIds; 
        }

        // CASO B: Mezcla de IDs y Flags
        List<string> explicitIds = inst.provincesToModify.Where(s => !s.StartsWith("[")).ToList();
        int playerHereFlags = inst.provincesToModify.Count(s => s.ToLower() == "[player_here]");
        int playerAwayFlags = inst.provincesToModify.Count(s => s.ToLower() == "[player_away]");

        List<string> results = new List<string>();

        // 1. Validar IDs explícitos: Solo cuentan si existen en el Manager
        foreach (var id in explicitIds) {
            if (allIds.Contains(id)) results.Add(id);
        }

        // 2. Validar Flags: ¿Hay suficientes candidatos para cubrir las flags pedidas?
        // (Restamos los explicitIds de los candidatos para no duplicar, igual que en ActiveModifier)
        int availablePresent = presentIds.Except(explicitIds).Count();
        int availableAbsent = absentIds.Except(explicitIds).Count();

        if (availablePresent >= playerHereFlags && availableAbsent >= playerAwayFlags) {
            // Si hay suficientes, añadimos "huecos" simbólicos para que el Count sea > 0
            for (int i = 0; i < playerHereFlags + playerAwayFlags; i++) results.Add("placeholder_id");
        } else {
            // Si faltan provincias para cumplir las flags, vaciamos resultados para invalidar
            return new List<string>();
        }

        return results;
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
        CheckModifiersExpiration();
        OnModifiersApplied?.Invoke(); // Avisamos que los valores ya han cambiado (actualiza totales en ParameterController primero)
        RefreshProjections(); // Proyectar nuevo día (ahora los totales están actualizados para la UI)
    
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
