using UnityEngine;
using UnityEngine.UI;

public class debug_Decision : MonoBehaviour
{
    public Button button;
    public SO_Decision decision;
    ModifierManager modifierManager;

    private void Start()
    {
        button.onClick.AddListener(SendModifiers);
        modifierManager = ServiceLocator.Get<ModifierManager>();
    }

    private void SendModifiers() {
        foreach (var mod in decision.modifiersArray) {
            modifierManager.ReadModifier(mod);
        }
    }
}
