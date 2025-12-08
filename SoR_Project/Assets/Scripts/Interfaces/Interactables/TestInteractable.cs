using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public debug_InteractablesUI ui;
    public bool leftClick;
    public bool leftClickHold;
    public bool hover;

    public void LeftClickInteract() {
        if(leftClick) ui.SetAction("Left click");
    }

    public void OnHover() {
        if(hover) ui.SetAction("Hover");
    }
    public void OnDeselect() {
        ui.SetAction("Deselected");
    }
    public void OnUnhover() {
        if(hover) ui.SetAction("Unhovered");
    }
}
