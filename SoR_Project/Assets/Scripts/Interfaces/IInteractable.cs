using UnityEngine;

public interface IInteractable
{
    public void LeftClickInteract();
    public void RightClickInteract();
    public void OnHover();
    public void OnLeftClickHold();
    public void OnRightClickHold();
}
