using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
    Transform transform { get; } // if something implements, needs to have transform property
}
