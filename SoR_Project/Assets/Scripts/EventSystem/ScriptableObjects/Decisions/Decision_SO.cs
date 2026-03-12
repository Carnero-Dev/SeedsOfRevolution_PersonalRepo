using UnityEngine;

[CreateAssetMenu(fileName = "Decision_SO", menuName = "SOR/EventSystem/Decisions")]
public class Decision_SO : ScriptableObject {
	[ShowOnly] public string eventId = "EVENT_ID"; //TODO: Recoger ID real del evento padre
	public string decisionId; 
	public string decisionTitle;
	[TextArea] public string decisionDescription;
    public ModifierInstructions[] modifiersArray;

	private void OnValidate() {
        // Sincronizamos la ID con todos los modificadores del array
        if (modifiersArray != null) {
            int i = 0;
            foreach (var mod in modifiersArray) {
                modifiersArray[i].decisionId = decisionId;
                if (string.IsNullOrEmpty(modifiersArray[i].customId)) modifiersArray[i].customId = $"{decisionId}_MOD_{i}";
                i++;
            }
        }
    }

}
