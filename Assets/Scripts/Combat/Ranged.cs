using UnityEngine;

public class Ranged : MonoBehaviour
{
    public WeaponData data;
    public GameObject bullet;
    public AudioClip attackSFX;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
