using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public debug_InteractablesUI ui;
    public bool leftClick;
    public bool rightClick;
    public bool leftClickHold;
    public bool rightClickHold;
    public bool hover;

    public void LeftClickInteract()
    {
        if(leftClick) ui.SetAction("Left click");
    }

    public void OnHover()
    {
        if(hover) ui.SetAction("Hover");
    }

    public void OnLeftClickHold()
    {
        if(leftClickHold){
            ui.SetAction("Left Click Hold");
            Debug.Log("INTERACTUA");
        }
    }

    public void OnRightClickHold()
    {
        if(rightClickHold) ui.SetAction("Right Click Hold");
    }

    public void RightClickInteract()
    {
        if(rightClick) ui.SetAction("Right Click Interact");
    }
}
