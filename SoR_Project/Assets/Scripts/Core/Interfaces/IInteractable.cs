using UnityEngine;

public interface IInteractable
{
    public void LeftClickInteract(RaycastHit hitinfo);
    public void OnHover(RaycastHit hitinfo);
    void OnDeselect();
    void OnUnhover();
}
