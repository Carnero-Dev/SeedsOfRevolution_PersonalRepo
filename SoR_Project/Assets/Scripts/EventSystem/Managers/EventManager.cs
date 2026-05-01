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

    [Header("Cooldown Defaults")]
    [SerializeField] private int _DEFAULT_EVENT_COOLDOWN = 7;
    [SerializeField] private int _DEFAULT_GLOBAL_COOLDOWN = 3;

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
        SO_Event currentEvent = _mapTemplate.eventsBatery.FirstOrDefault(e => e.eventStorage.eventId == parentEventId);
        status.hasTriggered = true;

        if (status != null && currentEvent != null) {
            // Marcar como disparado
            status.hasTriggered = true;

            // Lógica de Cooldown Custom
            //? Si es negativo (< 0), usamos el default del manager. Si no, el del evento.
            int cooldownToApply = currentEvent.eventStorage.cooldownDays <= 0 
                ? _DEFAULT_EVENT_COOLDOWN 
                : currentEvent.eventStorage.cooldownDays;

            status.availableDate = CalculateFutureDate(cooldownToApply);
        }

        // 4. Cooldown Global (se mantiene siempre)
        data.eventData.globalAvailableDate = CalculateFutureDate(_DEFAULT_GLOBAL_COOLDOWN); 

        // Limpiar cola
        _currentDayEventQueue.Remove(parentEventId);
        OnDecisionSelected?.Invoke();
    }

    public void RemoveFromQueue(string eventId) {
        _currentDayEventQueue.Remove(eventId);
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
            
            bool cooldownOk = status == null || IsAvailable(status.availableDate, today);
            bool uniqueOk = e.eventStorage.isUnique ? (status == null || !status.hasTriggered) : true;
            bool hasValidDecisions = e.eventStorage.decisions.Any(d => _modifierManager.IsDecisionValid(d));
            bool triggerDateOk;
            if (e.eventStorage.triggerDate != null && e.eventStorage.triggerDate.Split("/").Length == 3) {
                triggerDateOk = _timeManager.CurrentAbsDay >= _mapTemplate.calendarConfig.GetRelativeAbsDay(
                    int.Parse(e.eventStorage.triggerDate.Split('/')[0]), 
                    int.Parse(e.eventStorage.triggerDate.Split('/')[1]), 
                    int.Parse(e.eventStorage.triggerDate.Split('/')[2]));
            } else {
               triggerDateOk = true;
            }

            return cooldownOk && uniqueOk && hasValidDecisions && triggerDateOk;
        }).ToList();

        // Random event Roll 
        int roll = SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, 100);
        if (roll < 30 && eligibleEvents.Count > 0) {
            var ev = eligibleEvents[SeedRandom.RangeInt(SeedCategory.GLOBAL, 0, eligibleEvents.Count)];
            
            // Registrar el evento en la cola del día
            _currentDayEventQueue.Add(ev.eventStorage.eventId, ev);
        }
    }

    private bool IsAvailable(ExpirationDate cooldownDate, int currentAbsDay) {
        // Si el día actual es mayor o igual al día en que vence el cooldown, está disponible
        if (cooldownDate.absoluteDay <= 0) return true;
        return currentAbsDay >= cooldownDate.absoluteDay;
    }


    private ExpirationDate CalculateFutureDate(int daysToAdd) {
        // Día absoluto actual + días de espera = Día absoluto de disponibilidad
        return new ExpirationDate(_timeManager.CurrentAbsDay + daysToAdd);
    }
}