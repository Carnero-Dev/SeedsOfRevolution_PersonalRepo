using UnityEngine;

public interface IInteractable
{
    public void LeftClickInteract();
    public void OnHover();
    void OnDeselect();
    void OnUnhover();
}
