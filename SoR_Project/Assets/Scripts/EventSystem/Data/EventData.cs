using System;
using System.Collections.Generic;
[Serializable]
public class EventData {
    public List<EventStatus> eventStatuses = new();
    public ExpirationDate globalAvailableDate;
    public string activeEventId;
}