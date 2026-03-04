using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public debug_InteractablesUI ui;
    public bool leftClick;
    public bool leftClickHold;
    public bool hover;

    public void LeftClickInteract(RaycastHit hitinfo) {
        if(leftClick) ui.SetAction("Left click");
    }

    public void OnHover(RaycastHit hitinfo) {
        if(hover) ui.SetAction("Hover");
    }
    public void OnDeselect() {
        ui.SetAction("Deselected");
    }
    public void OnUnhover() {
        if(hover) ui.SetAction("Unhovered");
    }
}
