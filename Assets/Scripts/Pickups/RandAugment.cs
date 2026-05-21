using UnityEngine;

public class RandAugment : MonoBehaviour
{

    [SerializeField] private AugmentData[] augments;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 pos = transform.position;

        AugmentData augment = augments[Random.Range(0, augments.Length)];
        GameObject pickup = Instantiate(augment.pickupPrefab, pos, Quaternion.identity);
        pickup.GetComponent<AugmentPickup>().data = augment;
    }
}
