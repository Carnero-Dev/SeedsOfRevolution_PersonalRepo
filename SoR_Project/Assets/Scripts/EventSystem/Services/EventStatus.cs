using System;

[Serializable]
public class EventStatus {
    public string eventId;
    public ExpirationDate availableDate;
    public bool hasTriggered; // Para eventos únicos

    public EventStatus(string id, int startAbsDay = 0) {
        eventId = id;
        availableDate = new ExpirationDate(startAbsDay);
        hasTriggered = false;
    }
}