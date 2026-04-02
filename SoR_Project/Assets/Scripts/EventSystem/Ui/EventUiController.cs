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

    private CanvasGroup _canvasGroup;

    public void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();
        _eventManager = ServiceLocator.Get<EventManager>();
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        SetVisibility(false);
        _eventManager.OnDecisionSelected += CloseEventUi;
        _timeManager.OnHourPassedEvent += CheckForHourEvent;

        // Comprobar si hay eventos activos al cargar el juego
        if (_eventManager.GetActiveEventsQueue().Length > 0) {
            CheckForHourEvent();
        }
    }

    private void OnDisable() {
        _timeManager.OnHourPassedEvent -= CheckForHourEvent;
        _eventManager.OnDecisionSelected -= CloseEventUi;
    }

    private void SetVisibility(bool visible) {
        _canvasGroup.alpha = visible ? 1 : 0;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }

    private void CheckForHourEvent() {
        if(_eventManager.GetActiveEventsQueue().Length == 0) return;

        int currentHour = _data.gameTime.hour;
        var eventToTrigger = _eventManager.GetActiveEventsQueue().FirstOrDefault(e => e.eventStorage.triggerHour == currentHour);       
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
       SetVisibility(true);
    }

    public void CloseEventUi() {
         _timeManager.PauseReanudeTime(true);
        SetVisibility(false);
        foreach (Transform child in decisionContainer.transform) {
            Destroy(child.gameObject);
        }
        CheckForHourEvent(); // Por si hay más eventos a la misma hora
    }

}
