using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour {
    private GameData data => GameDataService.Current;
    private SO_MapTemplate _mapTemplate;
    private TimeManager _timeManager;
    private ModifierManager _modifierManager;

    public void Init(SO_MapTemplate map) {
        _mapTemplate = map;
        _timeManager = ServiceLocator.Get<TimeManager>();
        _modifierManager = ServiceLocator.Get<ModifierManager>();

        SyncEventData();

        _timeManager.OnDayPassedEvent += HandleDailyEventRoll;
        
        // Si al cargar había un evento a medias, reabrirlo
        if (!string.IsNullOrEmpty(data.eventData.activeEventId)) {
            //ResumeActiveEvent();
        }
    }

    private void SyncEventData() {
        if (data.eventData == null) data.eventData = new EventData();

        // Añadir eventos nuevos del SO que no estén en el guardado
        foreach (var ev in _mapTemplate.eventsBatery) {
            if (!data.eventData.eventStatuses.Any(s => s.eventId == ev.eventData.eventId)) {
                data.eventData.eventStatuses.Add(new EventStatus(ev.eventData.eventId));
            }
        }
    }

    private void HandleDailyEventRoll() {
        var storage = data.eventData;

        // 1. Check Cooldown Global
        if (!IsAvailable(storage.globalAvailableDate, data.gameTime)) return;

        // 2. Filtrar elegibles
        var eligibleEvents = _mapTemplate.eventsBatery.Where(e => {
            var status = storage.eventStatuses.Find(s => s.eventId == e.eventData.eventId);
            bool cooldownOk = IsAvailable(status.availableDate, data.gameTime);
            bool uniqueOk = !e.eventData.isUnique || !status.hasTriggered;
            
            return cooldownOk && uniqueOk;
        }).ToList();

        // 3. Roll por pesos
        int totalWeight = eligibleEvents.Sum(e => e.eventData.weight);
        int roll = SeedRandom.RangeInt(SeedCategory.GLOBAL,0, totalWeight);
        int cursor = 0;

        foreach (var ev in eligibleEvents) {
            cursor += ev.eventData.weight;
            if (roll <= cursor) {
                //TriggerEvent(ev);
                break;
            }
        }
    }
    private bool IsAvailable(ExpirationDate cooldownDate, TimeManagerData current) {
        if (current.year > cooldownDate.year) return true;
        if (current.year == cooldownDate.year && current.month > cooldownDate.month) return true;
        if (current.year == cooldownDate.year && current.month == cooldownDate.month && current.day >= cooldownDate.day) return true;
        return false;
    }

    public void OnDecisionSelected(SO_Decision decision) {
        //TODO: Aplicar efectos de la decisión
        foreach(var modifier in decision.modifiersArray) {
            _modifierManager.ReadModifier(modifier);
        }

        var status = data.eventData.eventStatuses.Find(s => s.eventId == data.eventData.activeEventId);
        status.hasTriggered = true;

        status.availableDate = CalculateFutureDate(30); 
        
        data.eventData.globalAvailableDate = CalculateFutureDate(7); 

        data.eventData.activeEventId = null;
        _timeManager.PauseReanudeTime(false);
    }

    private ExpirationDate CalculateFutureDate(int daysToAdd) {
        TimeManagerData current = data.gameTime;
        DateTime date = new DateTime(current.year, current.month, current.day);
        date = date.AddDays(daysToAdd);
        return new ExpirationDate(date.Day, date.Month, date.Year);
    }

    


}