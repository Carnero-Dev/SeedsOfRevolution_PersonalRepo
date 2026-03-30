using System;

[Serializable]
public class EventStatus {
    public string eventId;
    public ExpirationDate availableDate;
    public bool hasTriggered; // Para eventos únicos

    public EventStatus(string id) {
        eventId = id;
        availableDate = new ExpirationDate(0, 0, 0); // Disponible desde el inicio
        hasTriggered = false;
    }
}