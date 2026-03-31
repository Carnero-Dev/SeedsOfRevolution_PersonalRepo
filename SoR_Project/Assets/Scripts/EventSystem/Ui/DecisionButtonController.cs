using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionButtonController : MonoBehaviour
{
    private SO_Decision _decision;
    private string _parentEventId;
    private EventManager _eventManager;

    public void InitButton(SO_Decision decisionData, EventUiController uiController, string eventId) {
        _decision = decisionData;
        _parentEventId = eventId;
        this.GetComponent<Button>().onClick.AddListener(SendModifiers);
        this.GetComponentInChildren<TextMeshProUGUI>().text = decisionData.decisionTitle;
        _eventManager = ServiceLocator.Get<EventManager>();
    }

    private void SendModifiers() {
        _eventManager.TriggerDecision(_decision, _parentEventId);
    }
}
