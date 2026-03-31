using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour {
    private GameData data => GameDataService.Current;
    private SO_MapTemplate _mapTemplate;
    private TimeManager _timeManager;
    private ModifierManager _modifierManager;
    private Dictionary<string, SO_Event> _currentDayEventQueue = new Dictionary<string, SO_Event>();

    public Action OnResumeActiveEvent;
    public Action OnDecisionSelected;

    public void Init(SO_MapTemplate map) {
        _mapTemplate = map;
        _timeManager = ServiceLocator.Get<TimeManager>();
        _modifierManager = ServiceLocator.Get<ModifierManager>();

        SyncEventData();
        SaveSystem.OnCallSave += SyncToData; // Guardar Datos
        _timeManager.OnDayPassedEvent += HandleEventRoll;
        
        if (data.eventData.activeEventQueue.Count > 0) {
            OnResumeActiveEvent?.Invoke();
        }
    }

    public void OnDisable() {
        SaveSystem.OnCallSave -= SyncToData;
        _timeManager.OnDayPassedEvent -= HandleEventRoll;
    }

    public void TriggerDecision(SO_Decision decision, string parentEventId) {
        //TODO: Aplicar efectos de la decisión
        foreach(var modifier in decision.modifiersArray) {
            _modifierManager.ReadModifier(modifier);
        }
        // Guardar boolean de que el evento ya se ha disparado para no volver a mostrarlo si es único
        var status = data.eventData.eventStatuses.Find(s => s.eventId == parentEventId);
        status.hasTriggered = true;

        // Cálculo de cooldowns
        status.availableDate = CalculateFutureDate(7); 
        data.eventData.globalAvailableDate = CalculateFutureDate(3); 

        // Eliminar evento activo del guardado
       _currentDayEventQueue.Remove(parentEventId); //! QUE COÑO PASA AQUI QUE NO SE ELIMINAN
        OnDecisionSelected?.Invoke();
    }

    public SO_Event[] GetActiveEventsQueue() {
       return _currentDayEventQueue.Values.ToArray();
    }
    

    // Se asegura de que los los modificadores activos se gaurden en Json
	private void SyncToData() {
        data.eventData.activeEventQueue = _currentDayEventQueue.Keys.ToListPooled();
        Debug.Log("Modifier Synced");
    } 

    private void SyncEventData() {
        if (data.eventData == null) data.eventData = new EventData();

        // Añadir eventos nuevos del SO que no estén en el guardado
        foreach (var ev in _mapTemplate.eventsBatery) {
            if (!data.eventData.eventStatuses.Any(s => s.eventId == ev.eventStorage.eventId)) {
                data.eventData.eventStatuses.Add(new EventStatus(ev.eventStorage.eventId));
            }
        }
    }

    public void HandleEventRoll() {
        var storage = data.eventData;

        // Check Cooldown Global
        if (!IsAvailable(storage.globalAvailableDate, data.gameTime)) return;

        // Filtrar eventos elegibles
        var eligibleEvents = _mapTemplate.eventsBatery.Where(e => {
            var status = storage.eventStatuses.Find(s => s.eventId == e.eventStorage.eventId);
            bool cooldownOk = IsAvailable(status.availableDate, data.gameTime);
            bool uniqueOk = !e.eventStorage.isUnique || !status.hasTriggered;
            
            return cooldownOk && uniqueOk;
        }).ToList();

        // Random event Roll 
        int roll = SeedRandom.RangeInt(SeedCategory.GLOBAL,0, 100);
        if (roll < 30 && eligibleEvents.Count > 0) {
            var ev = eligibleEvents[SeedRandom.RangeInt(SeedCategory.GLOBAL,0, eligibleEvents.Count)];
            _currentDayEventQueue.Add(ev.eventStorage.eventId, ev);
        }
    }

    private bool IsAvailable(ExpirationDate cooldownDate, TimeManagerData current) {
        if (current.year > cooldownDate.year) return true;
        if (current.year == cooldownDate.year && current.month > cooldownDate.month) return true;
        if (current.year == cooldownDate.year && current.month == cooldownDate.month && current.day >= cooldownDate.day) return true;
        return false;
    }


    private ExpirationDate CalculateFutureDate(int daysToAdd) {
        TimeManagerData current = data.gameTime;
        DateTime date = new DateTime(current.year, current.month, current.day);
        date = date.AddDays(daysToAdd);
        return new ExpirationDate(date.Day, date.Month, date.Year);
    }
}