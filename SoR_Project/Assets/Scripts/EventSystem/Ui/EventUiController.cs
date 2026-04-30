using System.Linq;
using TMPro;
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
    private GameInitializer _gameInitializer;
    private ModifierManager _modifierManager;
    private GameData _data => GameDataService.Current;

    private CanvasGroup _eventCanvasGroup;
    [SerializeField] private CanvasGroup _interfaceCanvasGroup;

    public void Start() {
        _timeManager = ServiceLocator.Get<TimeManager>();
        _eventManager = ServiceLocator.Get<EventManager>();
        _gameInitializer = ServiceLocator.Get<GameInitializer>();
        _modifierManager = ServiceLocator.Get<ModifierManager>();
        _eventCanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        SetVisibility(false);
        _eventManager.OnDecisionSelected += CloseEventUi;
        _timeManager.OnHourPassedEvent += CheckForHourEvent;

        if (_gameInitializer.IsInitialized) CheckForActiveEvents();
         else _gameInitializer.OnGameInitialized += CheckForActiveEvents;
        
    }

    private void OnDisable() {
        _timeManager.OnHourPassedEvent -= CheckForHourEvent;
        _eventManager.OnDecisionSelected -= CloseEventUi;
        _gameInitializer.OnGameInitialized -= CheckForActiveEvents;
        
    }

    private void CheckForActiveEvents() {
        if(_eventManager.GetActiveEventsQueue().Length > 0) {
            CheckForHourEvent();
        }
    }

    private void SetVisibility(bool visible) {
        _eventCanvasGroup.alpha = visible ? 1 : 0;
        _eventCanvasGroup.interactable = visible;
        _eventCanvasGroup.blocksRaycasts = visible;

        if (_interfaceCanvasGroup != null) {
            _interfaceCanvasGroup.interactable = !visible;
        }
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
        var validDecisions = _event.eventStorage.decisions.Where(d => _modifierManager.IsDecisionValid(d)).ToArray();
    
        if(validDecisions.Length == 0) {
            _eventManager.RemoveFromQueue(_event.eventStorage.eventId);
            return;
        }
        _timeManager.PauseReanudeTime(false);
        _timeManager.SetTimeInputActive(false); 
            eventTitle.text = _event.eventStorage.title;
            eventDescription.text = _event.eventStorage.description;
            eventThumbnail.sprite = _event.eventStorage.thumbnail;

            foreach (var decision in validDecisions) {
                GameObject decisionObj = Instantiate(decisionPrefab, decisionContainer.transform);
                DecisionButtonController decisionButton = decisionObj.GetComponent<DecisionButtonController>();
                decisionButton.InitButton(decision, this, _event.eventStorage.eventId);
            }
        SetVisibility(true);
    }

    public void CloseEventUi() {
        SetVisibility(false);
        _timeManager.SetTimeInputActive(true); 
        _timeManager.PauseReanudeTime(true);
        foreach (Transform child in decisionContainer.transform) {
            Destroy(child.gameObject);
        }
        CheckForHourEvent(); // Por si hay más eventos a la misma hora
    }

}
