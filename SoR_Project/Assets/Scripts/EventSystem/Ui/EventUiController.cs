using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EventUiController : MonoBehaviour {

    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI eventDescription;
    public Image eventThumbnail;
    public GameObject decisionContainer;
    public GameObject decisionPrefab;

    private TimeManager _timeManager;
    private EventManager _eventManager;
     private GameData _data => GameDataService.Current;

    public void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();
        _eventManager = ServiceLocator.Get<EventManager>();
        this.gameObject.SetActive(false);
        _eventManager.OnResumeActiveEvent += CheckForHourEvent;
        _eventManager.OnDecisionSelected += CloseEventUi;
        _timeManager.OnHourPassedEvent += CheckForHourEvent;
    }

    private void OnDisable() {
        _timeManager.OnHourPassedEvent -= CheckForHourEvent;
        _eventManager.OnDecisionSelected -= CloseEventUi;
        _eventManager.OnResumeActiveEvent -= CheckForHourEvent;
    }

    private void CheckForHourEvent() {
        if(_eventManager.GetActiveEventsQueue().Length == 0) return;

        var eventToTrigger = _eventManager.GetActiveEventsQueue().FirstOrDefault(e => e.eventStorage.triggerHour == _data.gameTime.hour);
        if (eventToTrigger != null) {
            TriggerEventUi(eventToTrigger);
        }
    }

    private void TriggerEventUi(SO_Event _event) {
         _timeManager.PauseReanudeTime(false);
        eventTitle.text = _event.eventStorage.title;
        eventDescription.text = _event.eventStorage.description;
        eventThumbnail.sprite = _event.eventStorage.thumbnail;

        foreach (var decision in _event.eventStorage.decisions) {
            GameObject decisionObj = Instantiate(decisionPrefab, decisionContainer.transform);
            DecisionButtonController decisionButton = decisionObj.GetComponent<DecisionButtonController>();
            decisionButton.InitButton(decision, this, _event.eventStorage.eventId);
        }
        this.gameObject.SetActive(true);
    }

    public void CloseEventUi() {
         _timeManager.PauseReanudeTime(true);
        this.gameObject.SetActive(false);
        foreach (Transform child in decisionContainer.transform) {
            Destroy(child.gameObject);
        }
        CheckForHourEvent(); // Por si hay más eventos a la misma hora
    }

}
