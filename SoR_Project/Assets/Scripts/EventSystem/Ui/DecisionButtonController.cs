using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionButtonController : MonoBehaviour
{
    private EventUiController eventUiController;
    private SO_Decision decision;
    EventManager eventManager;

    public void InitButton(SO_Decision decisionData, EventUiController uiController) {
        decision = decisionData;
        eventUiController = uiController;
        this.GetComponent<Button>().onClick.AddListener(SendModifiers);
        this.GetComponentInChildren<TextMeshProUGUI>().text = decisionData.decisionTitle;
        eventManager = ServiceLocator.Get<EventManager>();
    }

    private void SendModifiers() {
        eventUiController.CloseEventUi();
        eventManager.OnDecisionSelected(decision);
    }
}
