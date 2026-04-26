using UnityEngine;

public class AugmentPickup : MonoBehaviour, IInteractable
{
    public playerController pc;
    public AugmentData augment;
    public Vector3 location;

    void Awake()
    {
        location = transform.position;
    }
    
    public void Interact(GameObject interactor)
    {
        pc = interactor.GetComponent<playerController>();
        PickupAugment();
    }

    public void PickupAugment()
    {
        
    }
}
