using UnityEngine;

public interface IInteractable {
    void Interact();
    Transform transform { get; } // if something implements, needs to have transform property
}
