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

    public void Start() => this.gameObject.SetActive(false);

    public void TriggerEventUi(SO_Event eventData) {
        eventTitle.text = eventData.eventData.title;
        eventDescription.text = eventData.eventData.description;
        eventThumbnail.sprite = eventData.eventData.thumbnail;

        foreach (var decision in eventData.eventData.decisions) {
            GameObject decisionObj = Instantiate(decisionPrefab, decisionContainer.transform);
            DecisionButtonController decisionButton = decisionObj.GetComponent<DecisionButtonController>();
            decisionButton.InitButton(decision, this);
        }
        this.gameObject.SetActive(true);
    }

    public void CloseEventUi() {
        this.gameObject.SetActive(false);
        foreach (Transform child in decisionContainer.transform) {
            Destroy(child.gameObject);
        }
    }

}
