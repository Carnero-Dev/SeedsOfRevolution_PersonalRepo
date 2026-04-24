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

    public Action OnDecisionSelected;

    public void Init(SO_MapTemplate map) {
        _mapTemplate = map;
        _timeManager = ServiceLocator.Get<TimeManager>();
        _modifierManager = ServiceLocator.Get<ModifierManager>();

        SyncEventData();
        SaveSystem.OnCallSave += SyncToData; // Guardar Datos
        _timeManager.OnDayPassedEvent += HandleEventRoll;
    }

    public void OnDisable() {
        SaveSystem.OnCallSave -= SyncToData;
        _timeManager.OnDayPassedEvent -= HandleEventRoll;
    }

    public void TriggerDecision(SO_Decision decision, string parentEventId) {
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
       _currentDayEventQueue.Remove(parentEventId);
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
            if (data.eventData.activeEventQueue.Contains(ev.eventStorage.eventId)) {
                _currentDayEventQueue.Add(ev.eventStorage.eventId, ev);
            }
        }
    }

    public void HandleEventRoll() {
        int today = _timeManager.CurrentAbsDay;

        // Check Cooldown Global
        if (!IsAvailable(data.eventData.globalAvailableDate, today)) return;

        // Filtrar eventos elegibles
        var eligibleEvents = _mapTemplate.eventsBatery.Where(e => {
            var status = data.eventData.eventStatuses.Find(s => s.eventId == e.eventStorage.eventId);
            
            // Si no existe status (evento nuevo), por defecto está disponible (absoluteDay = 0)
            bool cooldownOk = status == null || IsAvailable(status.availableDate, today);
            bool uniqueOk = e.eventStorage.isUnique ? (status == null || !status.hasTriggered) : true;
            
            return cooldownOk && uniqueOk;
        }).ToList();

        // Random event Roll 
        int roll = SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, 100);
        if (roll < 30 && eligibleEvents.Count > 0) {
            var ev = eligibleEvents[SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, eligibleEvents.Count)];
            
            // Registrar el evento en la cola del día
            _currentDayEventQueue.Add(ev.eventStorage.eventId, ev);

            // Si el evento tiene un cooldown tras ejecutarse, lo actualizamos así:
            // status.availableDate = CalculateFutureDate(ev.eventStorage.cooldownDays);
        }
    }

    private bool IsAvailable(ExpirationDate cooldownDate, int currentAbsDay) {
        // Si el día actual es mayor o igual al día en que vence el cooldown, está disponible
        return currentAbsDay >= cooldownDate.absoluteDay;
    }


    private ExpirationDate CalculateFutureDate(int daysToAdd) {
        // Día absoluto actual + días de espera = Día absoluto de disponibilidad
        return new ExpirationDate(_timeManager.CurrentAbsDay + daysToAdd);
    }
}