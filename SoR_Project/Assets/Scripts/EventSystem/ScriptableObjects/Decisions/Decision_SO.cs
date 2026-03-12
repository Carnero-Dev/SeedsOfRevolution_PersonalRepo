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
            for (int i = 0; i < modifiersArray.Length; i++) {
                // Inyectamos la ID del padre en el struct hijo
                modifiersArray[i].decisionId = decisionId;
                
                // Generamos un customId automático si está vacío
                if (string.IsNullOrEmpty(modifiersArray[i].customId)) {
                    modifiersArray[i].customId = $"{decisionId}_MOD_{i}";
                }
            }
        }
    }

}
